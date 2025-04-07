using DatabaseContext;
using DatabaseModel;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartEstate.DataAccess.Repositories
{
    public class UserPreferencesRepository : IUserPreferencesRepository
    {
        private readonly AppDbContext _dbContext;

        public UserPreferencesRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddFavoriteAsync(Guid userId, int flatId)
        {
            var favorite = new UserFavorite
            {
                UserId = userId,
                FlatId = flatId
            };

            await _dbContext.UserFavorites.AddAsync(favorite);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveFavoriteAsync(int flatId)
        {
            await _dbContext.UserFavorites
                .Where(f => f.FlatId == flatId)
                .ExecuteDeleteAsync();
        }

        public async Task<List<UserFavorite>> GetUserFavoritesAsync(Guid userId)
        {
            return await _dbContext.UserFavorites
                .Where(f => f.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> FavoriteExistsAsync(Guid userId, int flatId)
        {
            return await _dbContext.UserFavorites
                .AnyAsync(f => f.UserId == userId && f.FlatId == flatId);
        }

        public async Task AddComparisonAsync(Guid userId, int flatId1, int flatId2)
        {
            var comparison = new UserComparison
            {
                UserId = userId,
                FlatId1 = flatId1,
                FlatId2 = flatId2
            };

            await _dbContext.UserComparisons.AddAsync(comparison);
            await _dbContext.SaveChangesAsync();
        }

        public async Task RemoveComparisonAsync(int comparisonId)
        {
            await _dbContext.UserComparisons
                .Where(c => c.CompareId == comparisonId)
                .ExecuteDeleteAsync();
        }

        public async Task<List<UserComparison>> GetUserComparisonsAsync(Guid userId)
        {
            return await _dbContext.UserComparisons
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> ComparisonExistsAsync(Guid userId, int flatId1, int flatId2)
        {
            return await _dbContext.UserComparisons
                .AnyAsync(c => c.UserId == userId && 
                              ((c.FlatId1 == flatId1 && c.FlatId2 == flatId2) ||
                               (c.FlatId1 == flatId2 && c.FlatId2 == flatId1)));
        }
    }
}