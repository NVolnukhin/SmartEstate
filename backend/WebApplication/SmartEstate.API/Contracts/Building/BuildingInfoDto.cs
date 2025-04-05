namespace Presentation.Contracts.Building;

public record BuildingFullInfoDto(
    int BuildingId,
    string DeveloperName,
    int BuildingsCount,
    string DeveloperWebsite);