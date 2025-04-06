namespace Presentation.Contracts.InfrastructureInfo;

public record NearestShopInfo(
    string Name,
    int MinutesToShop,
    string NearestShopCoordinates);