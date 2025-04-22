// DataAccess/Repositories/BuildingsRepository.cs
using DatabaseContext;
using DatabaseModel;
using Microsoft.EntityFrameworkCore;
using Presentation.Contracts.Building;
using Presentation.Contracts.Flats;
using Presentation.Contracts.Metro;

namespace SmartEstate.DataAccess.Repositories;

public class BuildingsRepository : IBuildingsRepository
{
    private readonly AppDbContext _dbContext;

    public BuildingsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<BuildingMapDto>> GetAllBuildingsWithFlatsCountAsync()
    {
        return await _dbContext.Buildings
            
            .Select(b => new BuildingMapDto(
                b.BuildingId,
                b.Latitude,
                b.Longitude,
                _dbContext.Flats.Count(f => f.BuildingId == b.BuildingId),
                b.ResidentialComplex)
            )
            .ToListAsync();
    }

    public async Task<List<FlatShortInfoResponse>> GetFlatsByBuildingIdAsync(int buildingId)
    {
        var flatIds = await _dbContext.Flats
            .Where(f => f.BuildingId == buildingId)
            .Select(f => f.FlatId)
            .ToListAsync();

        if (!flatIds.Any())
            return new List<FlatShortInfoResponse>();

        var latestPrices = await _dbContext.PriceHistories
            .Where(ph => flatIds.Contains(ph.FlatId))
            .GroupBy(ph => ph.FlatId)
            .Select(g => new 
            {
                FlatId = g.Key,
                Price = g.OrderByDescending(ph => ph.ChangeDate)
                    .Select(ph => (decimal?)ph.Price)
                    .FirstOrDefault() ?? 0
            })
            .ToListAsync();

        var query = from flat in _dbContext.Flats
                    where flatIds.Contains(flat.FlatId)
                    join building in _dbContext.Buildings on flat.BuildingId equals building.BuildingId
                    join infra in _dbContext.InfrastructureInfos on building.BuildingId equals infra.BuildingId into infraGroup
                    from infra in infraGroup.DefaultIfEmpty()
                    select new
                    {
                        Flat = flat,
                        Building = building,
                        Infrastructure = infra,
                        Metro = infra.NearestMetro
                    };
        
        var results = await query.ToListAsync();
        
        return results.Select(x => new FlatShortInfoResponse(
            x.Flat.FlatId,
            x.Flat.Images.FirstOrDefault(),
            x.Flat.Square,
            x.Flat.Roominess,
            x.Flat.Floor,
            latestPrices.FirstOrDefault(p => p.FlatId == x.Flat.FlatId)?.Price ?? 0,
            x.Building.ResidentialComplex,
            x.Infrastructure != null ? new NearestMetroInfo(
                x.Metro?.Name ?? "Не указано",
                x.Infrastructure.MinutesToMetro,
                x.Metro != null 
                    ? $"{x.Metro.Latitude},{x.Metro.Longitude}" 
                    : "Координаты не указаны"
            ) : null
        )).ToList();
    }
}