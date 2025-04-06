using Presentation.Contracts.Flats;

namespace Presentation.Contracts.Favorites;

public record FavoriteResponse(
    int FavoriteId,
    int FlatId,
    FlatShortInfoResponse FlatInfo);
