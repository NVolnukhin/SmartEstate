using System.Security.Cryptography;

namespace SmartEstate.Email;

public class TokenGenerator : ITokenGenerator
{
    public string GenerateToken()
    {
        var randomBytes = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
    }
}