using DatabaseModel;

namespace SmartEstate.DataAccess.Repositories;

public interface IUsersRepository
{
    Task<User> GetById(Guid userId);
    public  Task<User?> GetByEmail(string email);
    public  Task<User?> GetByLogin(string login);

    public Task Add(User user);

    public Task UpdateEmail(Guid userId, string newEmail);
    public Task UpdateName(Guid userId, string newName);
    public Task UpdatePassword(Guid userId, string newPassword);

    public Task Delete(Guid userId);
}