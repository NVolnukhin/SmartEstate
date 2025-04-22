using Presentation.Contracts.Building;
using Presentation.Contracts.Flats;

namespace SmartEstate.DataAccess.Repositories;

public interface IBuildingsRepository
{
    Task<List<BuildingMapDto>> GetAllBuildingsWithFlatsCountAsync();
    Task<List<FlatShortInfoResponse>> GetFlatsByBuildingIdAsync(int buildingId);
}