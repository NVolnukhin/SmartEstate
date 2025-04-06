namespace Presentation.Contracts.InfrastructureInfo;

public record NearestSchoolInfo(
    string Name,
    int MinutesToSchool,
    string NearestSchoolCoordinates);