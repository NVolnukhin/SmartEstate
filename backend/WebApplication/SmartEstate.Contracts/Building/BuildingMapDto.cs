namespace Presentation.Contracts.Building;

public record BuildingMapDto(
    int BuildingId,
    double Latitude,
    double Longitude,
    int FlatsCount,
    string ResidentalComplex
);
