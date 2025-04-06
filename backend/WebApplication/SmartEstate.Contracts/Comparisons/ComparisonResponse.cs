using Presentation.Contracts.Flats;

namespace Presentation.Contracts.Comparisons;

public record ComparisonResponse(
    int ComparisonId,
    DateTime ComparedAt,
    FlatShortInfoResponse Flat1,
    FlatShortInfoResponse Flat2);
