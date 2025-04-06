using System.Diagnostics;
using System.Text;
using DatabaseContext;
using Microsoft.EntityFrameworkCore;
using Presentation.Contracts.Building;
using Presentation.Contracts.Developer;
using Presentation.Contracts.Flats;
using Presentation.Contracts.InfrastructureInfo;
using Presentation.Contracts.Metro;
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

    public async Task<List<FlatResponse>> GetAllFlatsAsync()
    {
        var flats = await _repository.GetAllFlats();
        var priceHistory = await _repository.GetLatestPrices();

        var buildingIds = flats.Select(f => f.BuildingId).Distinct().ToList();

        var buildings = await _dbContext.Buildings
            .Include(b => b.Developer)
            .Where(b => buildingIds.Contains(b.BuildingId))
            .ToDictionaryAsync(b => b.BuildingId);

        var infrastructureInfos = await _dbContext.InfrastructureInfos
            .Include(ii => ii.NearestMetro)
            .Where(ii => buildingIds.Contains(ii.BuildingId))
            .ToDictionaryAsync(ii => ii.BuildingId);

        var result = flats.Select(f =>
        {
            var latestPrice = priceHistory.First(ph => ph.FlatId == f.FlatId).Price;
            buildings.TryGetValue(f.BuildingId, out var building);
            infrastructureInfos.TryGetValue(f.BuildingId, out var infrastructure);
            var metro = infrastructure?.NearestMetro;

            return new FlatResponse(
                f.FlatId,
                f.Images,
                f.Square,
                f.Roominess,
                f.Floor,
                latestPrice,
                f.BuildingId,
                building != null ? new BuildingInfoDto(
                    building.ConstructionStatus,
                    building.FloorCount,
                    building.Address,
                    building.ResidentialComplex) : null,
                infrastructure != null ? new NearestMetroInfo(
                    metro?.Name ?? "Не указано",
                    infrastructure.MinutesToMetro,
                    metro != null ? $"{metro.Latitude},{metro.Longitude}" : "Координаты не указаны") : null
            );
        }).ToList();

        return result;
    }

    public async Task<List<FlatShortInfoResponse>> GetRandomFlatsAsync()
    {
        var flats = await _repository.GetRandomFlats(10);
        var priceHistory = await _repository.GetLatestPrices();
        var buildingIds = flats.Select(f => f.BuildingId).Distinct().ToList();

        var infrastructureInfos = await _dbContext.InfrastructureInfos
            .Include(ii => ii.NearestMetro)
            .Where(ii => buildingIds.Contains(ii.BuildingId))
            .ToDictionaryAsync(ii => ii.BuildingId);

        return flats.Select(f =>
        {
            var latestPrice = priceHistory.First(ph => ph.FlatId == f.FlatId).Price;
            infrastructureInfos.TryGetValue(f.BuildingId, out var infrastructure);
            var metro = infrastructure?.NearestMetro;

            return new FlatShortInfoResponse(
                f.FlatId,
                f.Images.FirstOrDefault(),
                f.Square,
                f.Roominess,
                f.Floor,
                latestPrice,
                infrastructure != null ? new NearestMetroInfo(
                    metro?.Name ?? "Не указано",
                    infrastructure.MinutesToMetro,
                    metro != null ? $"{metro.Latitude},{metro.Longitude}" : "Координаты не указаны") : null
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
            .FirstOrDefaultAsync(i => i.BuildingId == flat.BuildingId);

        string priceChartBase64 = await GetPriceChartBase64Async(flatId);
        
        return new FlatDetailsResponse(
            flat.FlatId,
            flat.Images,
            flat.Square,
            flat.Roominess,
            flat.Floor,
            flat.CianLink,
            flat.FinishType,
            price?.Price ?? 0,
            priceChartBase64,
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
            infrastructure != null ? new InfrastructureInfoDto(
                infrastructure.MinutesToMetro,
                infrastructure.MinutesToSchool,
                infrastructure.MinutesToKindergarten,
                infrastructure.MinutesToPharmacy,
                infrastructure.MinutesToShop) : null
        );
    }
    
    private async Task<string> GetPriceChartBase64Async(int flatId)
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = "python",
            Arguments = $"-c \"import sys; sys.path.append('../../GraphDrawer'); from price_history_plotter import get_price_chart_base64; print(get_price_chart_base64({flatId}), end='')\"",
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
            StandardOutputEncoding = Encoding.UTF8
        };

        using (var process = Process.Start(startInfo))
        {
            string output = await process.StandardOutput.ReadToEndAsync().ConfigureAwait(false);
            await process.WaitForExitAsync().ConfigureAwait(false);
            
            return output.Trim();
        }
    }

}
