namespace Presentation.Contracts.InfrastructureInfo;

public record NearestKindergartenInfo(
    string Name,
    int MinutesToKindergarten,
    string NearestKindergartenCoordinates);