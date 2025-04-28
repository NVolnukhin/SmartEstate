namespace Contracts.Developer;

public record DeveloperInfoDto(
    string Name,
    string? BuildingsCount,
    string? Website);