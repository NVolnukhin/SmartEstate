using DatabaseContext;
using DatabaseModel;
using Microsoft.EntityFrameworkCore;
using Presentation.Contracts.Building;
using Presentation.Contracts.Developer;
using Presentation.Contracts.InfrastructureInfo;
using Presentation.Contracts.Metro;
using SmartEstate.DataAccess.Repositories;

namespace Presentation.Endpoints;

using Microsoft.AspNetCore.Mvc;
using Contracts.Flats;

public static class FlatEndpoint
{
    public static IEndpointRouteBuilder MapFlatsEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapGet("flats", GetAllFlats);
        app.MapGet("flats/random", GetRandomFlats);
        app.MapGet("flats/{flatId:int}", GetFlatById);
        
        return app;
    }

    private static async Task<IResult> GetAllFlats(
        [FromServices] FlatsRepository repository,
        [FromServices] AppDbContext dbContext)
    {
        var flats = await repository.GetAllFlats();
        var priceHistory = await repository.GetLatestPrices();
        
        var buildingIds = flats.Select(f => f.BuildingId).Distinct().ToList();

        var buildings = await dbContext.Buildings
            .Include(b => b.Developer)
            .Where(b => buildingIds.Contains(b.BuildingId))
            .ToDictionaryAsync(b => b.BuildingId);

        var infrastructureInfos = await dbContext.InfrastructureInfos
            .Include(ii => ii.NearestMetro)
            .Where(ii => buildingIds.Contains(ii.BuildingId))
            .ToDictionaryAsync(ii => ii.BuildingId);

        var response = await Task.WhenAll(flats.Select(async f => 
        {
            // Получаем последнюю цену
            var latestPrice = priceHistory.First(ph => ph.FlatId == f.FlatId).Price;
    
            // Получаем информацию о здании
            buildings.TryGetValue(f.BuildingId, out var building);
    
            // Получаем информацию о метро
            infrastructureInfos.TryGetValue(f.BuildingId, out var infrastructure);
            var metroInfo = infrastructure?.NearestMetro;

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
                    metroInfo?.Name ?? "Не указано",
                    infrastructure.MinutesToMetro ,
                    metroInfo != null 
                        ? $"{metroInfo.Latitude},{metroInfo.Longitude}" 
                        : "Координаты не указаны") : null
            );
        }));
        
        return Results.Ok(response);
    }

    private static async Task<IResult> GetRandomFlats(
        [FromServices] FlatsRepository repository,
        [FromServices] AppDbContext dbContext)
    {
        var flats = await repository.GetRandomFlats(10);
        
        var priceHistory = await repository.GetLatestPrices();
        
        var buildingIds = flats.Select(f => f.BuildingId).Distinct().ToList();
        var infrastructureInfos = await dbContext.InfrastructureInfos
            .Include(ii => ii.NearestMetro)
            .Where(ii => buildingIds.Contains(ii.BuildingId))
            .ToDictionaryAsync(ii => ii.BuildingId);

        // Формируем ответ
        var response = flats.Select(f => 
        {
            var latestPrice = priceHistory.First(ph => ph.FlatId == f.FlatId).Price;
            
            infrastructureInfos.TryGetValue(f.BuildingId, out var infrastructure);
            var metroInfo = infrastructure?.NearestMetro;

            return new FlatShortInfoResponse(
                f.FlatId,
                f.Images[0],
                f.Square,
                f.Roominess,
                f.Floor,
                latestPrice,
                infrastructure != null ? new NearestMetroInfo(
                    metroInfo?.Name ?? "Не указано",
                    infrastructure.MinutesToMetro ,
                    metroInfo != null 
                        ? $"{metroInfo.Latitude},{metroInfo.Longitude}" 
                        : "Координаты не указаны") : null
            );
        }).ToList();

        return Results.Ok(response);
    }

    private static async Task<IResult> GetFlatById(
    [FromRoute] int flatId,
    [FromServices] FlatsRepository flatsRepository,
    [FromServices] AppDbContext dbContext)
{
    // Получаем квартиру по ID
    var flat = await flatsRepository.GetFlatById(flatId);
    if (flat is null)
        return Results.NotFound();

    // Получаем актуальную цену
    var price = await dbContext.PriceHistories
        .Where(ph => ph.FlatId == flatId)
        .OrderByDescending(ph => ph.ChangeDate)
        .FirstOrDefaultAsync();

    // Получаем информацию о здании и застройщике
    var building = await dbContext.Buildings
        .Include(b => b.Developer)
        .FirstOrDefaultAsync(b => b.BuildingId == flat.BuildingId);
    
    if (building is null)
        return Results.NotFound("Building not found");

    // Получаем информацию об инфраструктуре
    var infrastructure = await dbContext.InfrastructureInfos
        .FirstOrDefaultAsync(i => i.BuildingId == flat.BuildingId);

    // Формируем ответ
    var response = new FlatDetailsResponse(
        flat.FlatId,
        flat.Images,
        flat.Square,
        flat.Roominess,
        flat.Floor,
        flat.CianLink,
        flat.FinishType,
        price?.Price ?? 0,
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
            infrastructure.MinutesToShop) : null);

    return Results.Ok(response);
}
}