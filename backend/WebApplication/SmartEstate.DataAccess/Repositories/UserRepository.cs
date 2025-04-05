using DatabaseModel;
using SmartEstate.DataAccess.Repositories;
using System.Runtime.InteropServices;
using Microsoft.EntityFrameworkCore;

namespace DatabaseContext.Repositories;

public class UsersRepository : IUsersRepository
{
    private readonly AppDbContext _dbContext;

    public UsersRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User> GetById(Guid userId)
    {
        return await _dbContext.Users
                   .FirstOrDefaultAsync(u => u.UserId == userId) ?? throw new Exception("User not found");
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
    
    public async Task UpdateEmail(Guid userId, string newEmail)
    {
        await _dbContext.Users
            .Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(u => u
                .SetProperty(p => p.Email, newEmail));
    }

    public async Task UpdateName(Guid userId, string newName)
    {
        await _dbContext.Users
            .Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(u => u
                .SetProperty(p => p.Name, newName));
    }

    public async Task UpdatePassword(Guid userId, string newPasswordHash)
    {
        await _dbContext.Users
            .Where(u => u.UserId == userId)
            .ExecuteUpdateAsync(u => u
                .SetProperty(p => p.HashedPassword, newPasswordHash));
    }
    
    public async Task Delete(Guid userId)
    {
        await _dbContext.Users
            .Where(u => u.UserId == userId)
            .ExecuteDeleteAsync();
    }
}