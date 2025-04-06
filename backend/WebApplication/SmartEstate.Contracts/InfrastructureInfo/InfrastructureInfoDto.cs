namespace Presentation.Contracts.InfrastructureInfo;

public record InfrastructureInfoDto(
    int? MinutesToMetro,
    int? MinutesToSchool,
    int? MinutesToKindergarten,
    int? MinutesToPharmacy,
    int? MinutesToShop);
