namespace Presentation.Contracts.Metro;

public record NearestMetroInfo(
    string Name,
    int MinutesToMetro,
    string NearestMetroCoordinates);
