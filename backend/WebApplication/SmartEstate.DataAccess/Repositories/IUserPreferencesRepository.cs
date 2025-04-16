using DatabaseModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartEstate.DataAccess.Repositories
{
    public interface IUserPreferencesRepository
    {
        Task AddFavoriteAsync(Guid userId, int flatId);
        Task RemoveFavoriteAsync(int favoriteId);
        Task<List<UserFavorite>> GetUserFavoritesAsync(Guid userId);
        Task<bool> FavoriteExistsAsync(Guid userId, int flatId);

        Task AddComparisonAsync(Guid userId, int flatId1, int flatId2);
        Task RemoveComparisonAsync(int comparisonId);
        Task<List<UserComparison>> GetUserComparisonsAsync(Guid userId);
        Task<bool> ComparisonExistsAsync(Guid userId, int flatId1, int flatId2);
    }
}