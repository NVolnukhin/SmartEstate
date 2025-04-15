using Contracts.Developer;
using DatabaseModel;

namespace SmartEstate.Application.Services;

public interface IDeveloperService
{
    Task<List<ListDeveloperDto>> GetAllDevelopersAsync();
}