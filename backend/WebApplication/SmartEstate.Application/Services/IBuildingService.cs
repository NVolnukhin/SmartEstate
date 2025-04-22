using Presentation.Contracts.Building;
using Presentation.Contracts.Flats;

namespace SmartEstate.Application.Services;

public interface IBuildingService
{
    Task<List<BuildingMapDto>> GetAllBuildingsAsync();
    Task<List<FlatShortInfoResponse>> GetFlatsByBuildingIdAsync(int buildingId);
}
