using System;
using System.Linq;
using System.Threading.Tasks;
using DatabaseContext;
using DatabaseModel;
using DatabaseModels.RecoveryPassword;
using Microsoft.EntityFrameworkCore;

namespace SmartEstate.DataAccess.Repositories
{
    public class PasswordRecoveryTokenRepository : IPasswordRecoveryTokenRepository
    {
        private readonly AppDbContext _context;

        public PasswordRecoveryTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<PasswordRecoveryToken?> GetValidTokenAsync(string token)
        {
            return await _context.PasswordRecoveryTokens
                .FirstOrDefaultAsync(t => 
                    t.Token == token && 
                    !t.IsUsed && 
                    t.ExpiresAt > DateTime.UtcNow);
        }

        public async Task CreateTokenAsync(PasswordRecoveryToken token)
        {
            await _context.PasswordRecoveryTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task InvalidateUserTokensAsync(Guid userId)
        {
            var tokens = await _context.PasswordRecoveryTokens
                .Where(t => t.UserId == userId && !t.IsUsed)
                .ToListAsync();

            if (tokens.Any())
            {
                _context.PasswordRecoveryTokens.RemoveRange(tokens);
                await _context.SaveChangesAsync();
            }
        }
    }
}