namespace SmartEstate.Infrastructure;

public record PasswordHashingSettings
{
    public string ClientSalt { get; init; } = default!;
    public string ServerSaltPattern { get; init; } = default!;
}
