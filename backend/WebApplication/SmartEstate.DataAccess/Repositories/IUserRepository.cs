using DatabaseModel;

namespace SmartEstate.DataAccess.Repositories;

public interface IUsersRepository
{
    public  Task<User?> GetByEmail(string email);
    
    public  Task<User?> GetByLogin(string login);

    public Task Add(User user);

    public Task Update(Guid userId, string email, string name, string password);

    public Task Delete(Guid userId);
}