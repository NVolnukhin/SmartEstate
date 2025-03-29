namespace SmartEstate.Infrastructure;

public class JwtOptions
{
    public string SecretKey { get; set; } = string.Empty;
    public int ExpiresHours { get; set; } 
}

//TODO: JWT options and jwt provider
