using Contracts.InfrastructureInfo;
using DatabaseContext;
using DatabaseModel.Infrastucture;
using Microsoft.EntityFrameworkCore;

namespace SmartEstate.DataAccess.Repositories;

public class MetroRepository : IMetroRepository
{
    private readonly AppDbContext _dbContext;

    public MetroRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<MetroDto>> GetAllMetroStationsAsync()
    {
        return await _dbContext.Metro
            .AsNoTracking()
            .OrderBy(m => m.Name)
            .Select(m =>
                new MetroDto(m.Id, m.Name))
            .ToListAsync();
    }
}