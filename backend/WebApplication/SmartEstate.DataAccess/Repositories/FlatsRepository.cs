using DatabaseContext;
using DatabaseModel;
using Microsoft.EntityFrameworkCore;

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
}