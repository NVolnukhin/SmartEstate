namespace Contracts.Filters;

public record FlatFilterRequest(
    string? Order,
    decimal? MinPrice,
    decimal? MaxPrice,
    decimal? MinSquare,
    decimal? MaxSquare,
    int? MinFloor,
    int? MaxFloor,
    int? MinFloorCount,
    int? MaxFloorCount,
    int? MaxMetroTime,
    List<int>? Roominess,
    List<int>? MetroStations,
    List<int>? Developers,
    List<string>? BuildingStatus
    );
