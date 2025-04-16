using Contracts.Developer;
using Contracts.InfrastructureInfo;
using DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace SmartEstate.DataAccess.Repositories;

public class DeveloperRepository : IDeveloperRepository
{
    private readonly AppDbContext _dbContext;

    public DeveloperRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<List<ListDeveloperDto>> GetAllDevelopersAsync()
    {
        return await _dbContext.Developers
            .AsNoTracking()
            .OrderBy(d => d.Name)
            .Select(d =>
                new ListDeveloperDto(d.DeveloperId, d.Name))
            .ToListAsync();
    }
}