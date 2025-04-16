using Presentation.Contracts.Building;
using Presentation.Contracts.Metro;

namespace Presentation.Contracts.Flats;

public record FlatResponse(
    int FlatId,
    string[] Images,
    decimal Square,
    int Roominess,
    int Floor,
    decimal Price,
    int BuildingId,
    BuildingInfoDto? Building,
    NearestMetroInfo? NearestMetro);
