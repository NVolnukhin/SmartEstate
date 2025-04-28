namespace SmartEstate.Infrastructure;

public record EmailEncryptionSettings
{
    public string EncryptionKey { get; init; } = null!;
    public string IV { get; init; } = null!;
}
