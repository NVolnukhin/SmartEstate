namespace Presentation.Contracts.InfrastructureInfo;

public record NearestPharmacyInfo(
    string Name,
    int? MinutesToPharmacy,
    string? NearestPharmacyCoordinates);