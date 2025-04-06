using DatabaseContext;
using DatabaseModel;
using Microsoft.EntityFrameworkCore;
using Presentation.Contracts.Flats;
using Presentation.Contracts.Metro;

namespace SmartEstate.DataAccess.Repositories;



public class FlatsRepository : IFlatsRepository
{
    private readonly AppDbContext _dbContext;

    public FlatsRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<Flat>> GetAllFlats()
    {
        return await _dbContext.Flats
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<List<Flat>> GetRandomFlats(int count)
    {
        return await _dbContext.Flats
            .AsNoTracking()
            .OrderBy(x => Guid.NewGuid())
            .Take(count)
            .ToListAsync();
    }

    public async Task<Flat?> GetFlatById(int flatId)
    {
        return await _dbContext.Flats
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.FlatId == flatId);
    }
    
    public async Task<IEnumerable<PriceHistory>> GetPriceHistory()
    {
        return await _dbContext.PriceHistories
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<IEnumerable<PriceHistory>> GetLatestPrices()
    {
        return await _dbContext.PriceHistories
            .GroupBy(ph => ph.FlatId)
            .Select(g => g.OrderByDescending(ph => ph.ChangeDate).FirstOrDefault())
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<List<FlatShortInfoResponse>> GetFlatsWithDetails(List<int> flatIds)
    {
        if (!flatIds.Any()) return new List<FlatShortInfoResponse>();

        // Получаем latestPrices отдельно
        var latestPrices = await _dbContext.PriceHistories
            .Where(ph => flatIds.Contains(ph.FlatId))
            .GroupBy(ph => ph.FlatId)
            .Select(g => new 
            {
                FlatId = g.Key,
                Price = g.OrderByDescending(ph => ph.ChangeDate)
                    .Select(ph => (decimal?)ph.Price) // в случае если Price decimal
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

// Подставляем latestPrice уже в памяти
        return results.Select(x =>
        {
            var price = latestPrices.FirstOrDefault(p => p.FlatId == x.Flat.FlatId)?.Price ?? 0;

            return new FlatShortInfoResponse(
                x.Flat.FlatId,
                x.Flat.Images.FirstOrDefault() ?? "no-image.jpg",
                x.Flat.Square,
                x.Flat.Roominess,
                x.Flat.Floor,
                price,
                x.Infrastructure != null ? new NearestMetroInfo(
                    x.Metro?.Name ?? "Не указано",
                    x.Infrastructure.MinutesToMetro,
                    x.Metro != null 
                        ? $"{x.Metro.Latitude},{x.Metro.Longitude}" 
                        : "Координаты не указаны"
                ) : null
            );
        }).ToList();

    }

}