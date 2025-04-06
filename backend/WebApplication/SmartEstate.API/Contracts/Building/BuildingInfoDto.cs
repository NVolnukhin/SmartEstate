namespace Presentation.Contracts.Building;

public record BuildingInfoDto(
    string Status,
    int FloorCount,
    string Address,
    string ResidentialComplex);