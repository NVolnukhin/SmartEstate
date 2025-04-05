using DatabaseModel;
using SmartEstate.DataAccess.Repositories;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace DatabaseContext.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _dbContext;

    public UsersRepository(AppDbContext dbContext/*, IMapper mapper*/)
    {
        _dbContext = dbContext;
    }

    public async Task<List<User>> GetAllUsers()
    {
        return await _dbContext.Users
            .AsNoTracking()
            .OrderBy(u => u.Email)
            .ToListAsync();
    }
    
    public async Task<User?> GetByEmail(string email)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetByLogin(string login)
    {
        return await _dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Login == login);
    }
    
    public async Task Add(User user)
    {
        await _dbContext.Users.AddAsync(user);
        await _dbContext.SaveChangesAsync();
    }
    
    public async Task Update(Guid userId, string name, string email, string password)
    {
        await _dbContext.Users
            .Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(u => u
                    .SetProperty(p => p.Name, name)
                    .SetProperty(p => p.Email, email)
                    .SetProperty(p => p.HashedPassword, password)
            );
    }
    
    public async Task Delete(Guid userId)
    {
        await _dbContext.Users
            .Where(u => u.UserId == userId)
            .ExecuteDeleteAsync();
    }
}