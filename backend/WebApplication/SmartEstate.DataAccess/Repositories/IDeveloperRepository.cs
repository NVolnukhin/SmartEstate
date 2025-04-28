using Contracts.Developer;

namespace SmartEstate.DataAccess.Repositories;

public interface IDeveloperRepository
{
    Task<List<ListDeveloperDto>> GetAllDevelopersAsync();
}