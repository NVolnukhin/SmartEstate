using DatabaseModel;

namespace SmartEstate.Application.Interfaces;

public interface IJwtProvider
{
    public string GenerateToken(User user);
}