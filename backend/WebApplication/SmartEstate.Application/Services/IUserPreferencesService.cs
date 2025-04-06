using Presentation.Contracts.Comparisons;
using Presentation.Contracts.Favorites;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartEstate.ApplicationServices
{
    public interface IUserPreferencesService
    {
        // Favorites
        Task AddFavoriteAsync(Guid userId, AddFavoriteRequest request);
        Task RemoveFavoriteAsync(Guid userId, int favoriteId);
        Task<List<FavoriteResponse>> GetUserFavoritesAsync(Guid userId);

        // Comparisons
        Task AddComparisonAsync(Guid userId, AddComparisonRequest request);
        Task RemoveComparisonAsync(Guid userId, int comparisonId);
        Task<List<ComparisonResponse>> GetUserComparisonsAsync(Guid userId);
    }
}