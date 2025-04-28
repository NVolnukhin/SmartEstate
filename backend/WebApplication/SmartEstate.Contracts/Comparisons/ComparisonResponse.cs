using Presentation.Contracts.Flats;

namespace Presentation.Contracts.Comparisons;

public record ComparisonResponse(
    int ComparisonId,
    FlatShortInfoResponse Flat1,
    FlatShortInfoResponse Flat2);
