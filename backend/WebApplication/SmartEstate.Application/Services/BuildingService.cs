using Presentation.Contracts.Building;
using Presentation.Contracts.Flats;
using SmartEstate.DataAccess.Repositories;

namespace SmartEstate.Application.Services;

public class BuildingService : IBuildingService
{
    private readonly IBuildingsRepository _repository;

    public BuildingService(IBuildingsRepository repository)
    {
        _repository = repository;
    }

    public async Task<List<BuildingMapDto>> GetAllBuildingsAsync()
    {
        return await _repository.GetAllBuildingsWithFlatsCountAsync();
    }

    public async Task<List<FlatShortInfoResponse>> GetFlatsByBuildingIdAsync(int buildingId)
    {
        return await _repository.GetFlatsByBuildingIdAsync(buildingId);
    }
}
