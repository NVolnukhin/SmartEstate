using Presentation.Contracts.Metro;

namespace Presentation.Contracts.Flats;

public record FlatShortInfoResponse(
    int FlatId,
    string MainImage,
    decimal Square,
    int Roominess,
    int Floor,
    decimal Price,
    NearestMetroInfo? NearestMetro);