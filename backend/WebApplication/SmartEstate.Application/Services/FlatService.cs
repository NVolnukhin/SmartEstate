using Contracts;
using Contracts.Filters;
using Contracts.Flats;
using DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Presentation.Contracts.Building;
using Presentation.Contracts.Developer;
using Presentation.Contracts.Flats;
using Presentation.Contracts.InfrastructureInfo;
using Presentation.Contracts.Metro;
using Presentation.Contracts.Price;
using SmartEstate.DataAccess.Repositories;

namespace SmartEstate.Application.Services;

public class FlatService : IFlatService
{
    private readonly IFlatsRepository _repository;
    private readonly AppDbContext _dbContext;

    public FlatService(IFlatsRepository repository, AppDbContext dbContext)
    {
        _repository = repository;
        _dbContext = dbContext;
    }
    
    public async Task<PagedResponse<FlatResponse>> GetAllFlatsAsync(int page = 1, int pageSize = 15, FlatFilterRequest? filters = null)
    {

        var latestPricesQuery = from ph in _dbContext.PriceHistories
                            group ph by ph.FlatId into g
                            select new
                            {
                                FlatId = g.Key,
                                Price = g.OrderByDescending(ph => ph.ChangeDate)
                                        .Select(ph => ph.Price)
                                        .FirstOrDefault()
                            };

        var latestPrices = await latestPricesQuery.ToDictionaryAsync(x => x.FlatId, x => x.Price);
        var flatsQuery = _dbContext.Flats.AsQueryable();

        if (filters != null)
        {
            if (filters.MinPrice.HasValue || filters.MaxPrice.HasValue)
            {
                var filteredFlatIds = latestPrices
                    .Where(x => (!filters.MinPrice.HasValue || x.Value >= filters.MinPrice.Value) &&
                            (!filters.MaxPrice.HasValue || x.Value <= filters.MaxPrice.Value))
                    .Select(x => x.Key)
                    .ToList();

                flatsQuery = flatsQuery.Where(f => filteredFlatIds.Contains(f.FlatId));
            }

            if (filters.Roominess != null && filters.Roominess.Any())
            {
                flatsQuery = flatsQuery.Where(f => filters.Roominess.Contains(f.Roominess) || 
                    (f.Roominess >= 4 && filters.Roominess.Contains(4)));
            }

            if (filters.MinSquare.HasValue)
            {
                flatsQuery = flatsQuery.Where(f => f.Square >= filters.MinSquare.Value);
            }
            if (filters.MaxSquare.HasValue)
            {
                flatsQuery = flatsQuery.Where(f => f.Square <= filters.MaxSquare.Value);
            }

            if (filters.MinFloor.HasValue)
            {
                flatsQuery = flatsQuery.Where(f => f.Floor >= filters.MinFloor.Value);
            }
            if (filters.MaxFloor.HasValue)
            {
                flatsQuery = flatsQuery.Where(f => f.Floor <= filters.MaxFloor.Value);
            }

            if (filters.MetroStations != null && filters.MetroStations.Any())
            {
                var buildingIdsWithMetroStations = await _dbContext.InfrastructureInfos
                    .Where(ii => ii.NearestMetroId != null && 
                                filters.MetroStations.Contains(ii.NearestMetroId.Value))
                    .Select(ii => ii.BuildingId)
                    .Distinct()
                    .ToListAsync();

                flatsQuery = flatsQuery.Where(f => buildingIdsWithMetroStations.Contains(f.BuildingId));
            }

            if (filters.MaxMetroTime.HasValue)
            {
                var buildingIdsWithMetroTime = await _dbContext.InfrastructureInfos
                        .Where(ii => ii.MinutesToMetro != null && 
                                    ii.MinutesToMetro <= filters.MaxMetroTime.Value)
                    .Select(ii => ii.BuildingId)
                    .Distinct()
                    .ToListAsync();

                flatsQuery = flatsQuery.Where(f => buildingIdsWithMetroTime.Contains(f.BuildingId));
            }

            if (filters.MinFloorCount.HasValue || filters.MaxFloorCount.HasValue)
            {
                var buildingsQuery = _dbContext.Buildings.AsQueryable();

                if (filters.MinFloorCount.HasValue)
                {
                    buildingsQuery = buildingsQuery.Where(b => b.FloorCount >= filters.MinFloorCount.Value);
                }
                if (filters.MaxFloorCount.HasValue)
                {
                    buildingsQuery = buildingsQuery.Where(b => b.FloorCount <= filters.MaxFloorCount.Value);
                }

                var filteredBuildingIds = await buildingsQuery
                    .Select(b => b.BuildingId)
                    .ToListAsync();

                flatsQuery = flatsQuery.Where(f => filteredBuildingIds.Contains(f.BuildingId));
            }

            if (filters.BuildingStatus != null && filters.BuildingStatus.Any())
            {
                var buildingIdsWithStatus = await _dbContext.Buildings
                    .Where(b => filters.BuildingStatus.Contains(b.ConstructionStatus))
                    .Select(b => b.BuildingId)
                    .ToListAsync();

                flatsQuery = flatsQuery.Where(f => buildingIdsWithStatus.Contains(f.BuildingId));
            }
        }

        var totalCount = await flatsQuery.CountAsync();

        var paginatedFlats = await flatsQuery
            .OrderBy(f => f.FlatId)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var buildingIds = paginatedFlats.Select(f => f.BuildingId).Distinct().ToList();

        var buildings = await _dbContext.Buildings
            .Include(b => b.Developer)
            .Where(b => buildingIds.Contains(b.BuildingId))
            .ToDictionaryAsync(b => b.BuildingId);

        var infrastructureInfos = await _dbContext.InfrastructureInfos
            .Include(ii => ii.NearestMetro)
            .Where(ii => buildingIds.Contains(ii.BuildingId))
            .ToDictionaryAsync(ii => ii.BuildingId);

        var result = paginatedFlats.Select(f =>
        {
            buildings.TryGetValue(f.BuildingId, out var building);
            infrastructureInfos.TryGetValue(f.BuildingId, out var infrastructure);
            var metro = infrastructure?.NearestMetro;

            return new FlatResponse(
                f.FlatId,
                f.Images,
                f.Square,
                f.Roominess,
                f.Floor,
                latestPrices.TryGetValue(f.FlatId, out var price) ? price : 0,
                f.BuildingId,
                building != null ? new BuildingInfoDto(
                    building.ConstructionStatus,
                    building.FloorCount,
                    building.Address,
                    building.ResidentialComplex) : null,
                infrastructure != null ? new NearestMetroInfo(
                    metro?.Name ?? "Не указано",
                    infrastructure.MinutesToMetro,
                    metro != null ? $"{metro.Latitude}, {metro.Longitude}" : "Координаты не указаны") : null
            );
        }).ToList();

        return new PagedResponse<FlatResponse>(result, totalCount, page, pageSize);
    }
    
    public async Task<List<FlatShortInfoResponse>> GetRandomFlatsAsync(int count = 10)
    {
        var flats = await _repository.GetRandomFlats(count);
        var priceHistory = await _repository.GetLatestPrices();
        var buildingIds = flats.Select(f => f.BuildingId).Distinct().ToList();

        var infrastructureInfos = await _dbContext.InfrastructureInfos
            .Include(ii => ii.NearestMetro)
            .Where(ii => buildingIds.Contains(ii.BuildingId))
            .ToDictionaryAsync(ii => ii.BuildingId);

        var buildings = await _dbContext.Buildings
            .Where(b => buildingIds.Contains(b.BuildingId))
            .ToListAsync();
            
            
        
        return flats.Select( f =>
        {
            var latestPrice = priceHistory.First(ph => ph.FlatId == f.FlatId).Price;
            infrastructureInfos.TryGetValue(f.BuildingId, out var infrastructure);
            var metro = infrastructure?.NearestMetro;
            
            var residentialComplex = buildings.
                Where(b => b.BuildingId == f.BuildingId)
                .Select(b => b.ResidentialComplex)
                .FirstOrDefault();

                
            return new FlatShortInfoResponse(
                f.FlatId,
                f.Images.FirstOrDefault(),
                f.Square,
                f.Roominess,
                f.Floor,
                latestPrice,
                residentialComplex ?? "Не указано",
                infrastructure != null ? new NearestMetroInfo(
                    metro?.Name ?? "Не указано",
                    infrastructure.MinutesToMetro,
                    metro != null ? $"{metro.Latitude}, {metro.Longitude}" : "Координаты не указаны") : null
            );
        }).ToList();
    }

    public async Task<FlatDetailsResponse?> GetFlatDetailsByIdAsync(int flatId)
    {
        var flat = await _repository.GetFlatById(flatId);
        if (flat == null)
            return null;

        var price = await _dbContext.PriceHistories
            .Where(ph => ph.FlatId == flatId)
            .OrderByDescending(ph => ph.ChangeDate)
            .FirstOrDefaultAsync();

        var building = await _dbContext.Buildings
            .Include(b => b.Developer)
            .FirstOrDefaultAsync(b => b.BuildingId == flat.BuildingId);
        if (building == null)
            return null;

        var infrastructure = await _dbContext.InfrastructureInfos
            .Include(infrastructureInfo => infrastructureInfo.NearestMetro)
            .Include(infrastructureInfo => infrastructureInfo.NearestSchool)
            .Include(infrastructureInfo => infrastructureInfo.NearestKindergarten)
            .Include(infrastructureInfo => infrastructureInfo.NearestShop)
            .Include(infrastructureInfo => infrastructureInfo.NearestPharmacy)
            .FirstOrDefaultAsync(i => i.BuildingId == flat.BuildingId);

        var metro = infrastructure?.NearestMetro;
        var school = infrastructure?.NearestSchool;
        var kindergarten = infrastructure?.NearestKindergarten;
        var shop = infrastructure?.NearestShop;
        var pharmacy = infrastructure?.NearestPharmacy;
        
        var priceHistories = await _dbContext.PriceHistories
            .Where(ph => ph.FlatId == flatId)
            .OrderBy(ph => ph.ChangeDate)
            .Select(ph => new PriceDto(ph.Price, ph.ChangeDate))
            .ToArrayAsync();

        return new FlatDetailsResponse(
            flat.FlatId,
            flat.Images,
            flat.Square,
            flat.Roominess,
            flat.Floor,
            flat.CianLink,
            flat.FinishType,
            price?.Price ?? 0,
            priceHistories,
            flat.BuildingId,
            building.Developer.DeveloperId,
            infrastructure?.InfrastructureInfoId ?? 0,
            new BuildingInfoDto(
                building.ConstructionStatus,
                building.FloorCount,
                building.Address,
                building.ResidentialComplex),
            new DeveloperInfoDto(
                building.Developer.Name,
                building.Developer.BuildingsCount,
                building.Developer.Website),
            infrastructure != null
                ? new NearestMetroInfo(
                    metro?.Name ?? "Не указано",
                    infrastructure.MinutesToMetro,
                    metro != null ? $"{metro.Latitude}, {metro.Longitude}" : "Координаты не указаны")
                : null,
            infrastructure != null
                ? new NearestSchoolInfo(
                    school?.Name ?? "Не указано",
                    infrastructure.MinutesToSchool,
                    metro != null ? $"{school.Latitude}, {school.Longitude}" : "Координаты не указаны")
                : null,
            infrastructure != null
                ? new NearestKindergartenInfo(
                    kindergarten?.Name ?? "Не указано",
                    infrastructure.MinutesToKindergarten,
                    metro != null ? $"{kindergarten.Latitude}, {kindergarten.Longitude}" : "Координаты не указаны")
                : null,
            infrastructure != null
                ? new NearestShopInfo(
                    shop?.Name ?? "Не указано",
                    infrastructure.MinutesToShop,
                    shop != null ? $"{shop.Latitude}, {shop.Longitude}" : "Координаты не указаны")
                : null,
            infrastructure != null
                ? new NearestPharmacyInfo(
                    pharmacy?.Name ?? "Не указано",
                    infrastructure.MinutesToPharmacy,
                    pharmacy != null ? $"{pharmacy.Latitude}, {pharmacy.Longitude}" : "Координаты не указаны")
                : null
        );
    }
}
