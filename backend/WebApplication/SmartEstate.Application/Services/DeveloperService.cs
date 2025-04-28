using Contracts.Developer;
using SmartEstate.DataAccess.Repositories;

namespace SmartEstate.Application.Services;

public class DeveloperService : IDeveloperService
{

    private readonly IDeveloperRepository _developerRepository;
    
    public DeveloperService(IDeveloperRepository developerRepository)
    {
        _developerRepository = developerRepository;
    }
    
    public async Task<List<ListDeveloperDto>> GetAllDevelopersAsync()
    {
        return await _developerRepository.GetAllDevelopersAsync();
    }
}