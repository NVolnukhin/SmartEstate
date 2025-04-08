namespace Presentation.Contracts.Users;

public record UserInfoResponse(
    string Login,
    string Name,
    string Email);