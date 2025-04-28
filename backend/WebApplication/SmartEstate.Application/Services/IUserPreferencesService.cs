using Presentation.Contracts.Comparisons;
using Presentation.Contracts.Favorites;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;

namespace SmartEstate.ApplicationServices
{
    public interface IUserPreferencesService
    {
        // Favorites
        Task AddFavoriteAsync(Guid userId, AddFavoriteRequest request);
        Task RemoveFavoriteAsync(Guid userId, int favoriteId);
        Task<List<FavoriteResponse>> GetUserFavoritesAsync(Guid userId);
        Task<PagedResponse<FavoriteResponse>>GetPagedUserFavoritesAsync(Guid userId, int page, int pageSize);

        // Comparisons
        Task AddComparisonAsync(Guid userId, AddComparisonRequest request);
        Task RemoveComparisonAsync(Guid userId, int comparisonId);
        Task<List<ComparisonResponse>> GetUserComparisonsAsync(Guid userId);
        Task<PagedResponse<ComparisonResponse>>GetPagedUserComparisonsAsync(Guid userId, int page, int pageSize);
    }
}