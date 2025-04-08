using DatabaseModel;
using Presentation.Contracts.Comparisons;
using Presentation.Contracts.Favorites;
using SmartEstate.DataAccess.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Presentation.Contracts.Building;
using Presentation.Contracts.Flats;
using Presentation.Contracts.Metro;

namespace SmartEstate.ApplicationServices
{
    public class UserPreferencesService : IUserPreferencesService
    {
        private readonly IUserPreferencesRepository _userPreferencesRepo;
        private readonly IFlatsRepository _flatsRepo;
        private readonly IUsersRepository _usersRepo;

        public UserPreferencesService(
            IUserPreferencesRepository userPreferencesRepo,
            IFlatsRepository flatsRepo,
            IUsersRepository usersRepo)
        {
            _userPreferencesRepo = userPreferencesRepo;
            _flatsRepo = flatsRepo;
            _usersRepo = usersRepo;
        }

        public async Task AddFavoriteAsync(Guid userId, AddFavoriteRequest request)
        {
            await _usersRepo.GetById(userId); // Проверка существования пользователя

            if (await _flatsRepo.GetFlatById(request.FlatId) == null)
                throw new ArgumentException("Flat not found");

            if (await _userPreferencesRepo.FavoriteExistsAsync(userId, request.FlatId))
                throw new InvalidOperationException("Flat already in favorites");

            await _userPreferencesRepo.AddFavoriteAsync(userId, request.FlatId);
        }

        public async Task RemoveFavoriteAsync(Guid userId, int flatId)
        {
            var userFavorites = await _userPreferencesRepo.GetUserFavoritesAsync(userId);
            foreach (var f in userFavorites)
            {
                Console.WriteLine($"Favorite from list: {f.FavoriteId} ({f.FavoriteId.GetType()})");
            }
            var favorite = userFavorites.FirstOrDefault(f => f.FlatId == flatId);
            
            if (favorite == null)
                throw new UnauthorizedAccessException("Favorite not found or doesn't belong to user");

            await _userPreferencesRepo.RemoveFavoriteAsync(flatId);
        }

        public async Task<List<FavoriteResponse>> GetUserFavoritesAsync(Guid userId)
        {
            var favorites = await _userPreferencesRepo.GetUserFavoritesAsync(userId);
            if (!favorites.Any()) 
                return new List<FavoriteResponse>();

            var flatIds = favorites.Select(f => f.FlatId).ToList();
            var flatsInfo = await _flatsRepo.GetFlatsWithDetails(flatIds);

            return favorites.Select(f => 
            {
                var flatInfo = flatsInfo.FirstOrDefault(x => x.FlatId == f.FlatId);
                return new FavoriteResponse(
                    f.FavoriteId,
                    f.FlatId,
                    flatInfo ?? throw new Exception($"Flat info not found for ID {f.FlatId}"));
            }).ToList();
        }

        public async Task<PagedResponse<FavoriteResponse>>GetPagedUserFavoritesAsync(Guid userId, int page, int pageSize)
        {
            var favorites = await _userPreferencesRepo.GetUserFavoritesAsync(userId);
            
            var paginatedFavorites = favorites
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            var flatIds = favorites.Select(f => f.FlatId).ToList();
            var flatsInfo = await _flatsRepo.GetFlatsWithDetails(flatIds);

            var result = paginatedFavorites.Select(f =>
            {
                var flatInfo = flatsInfo.FirstOrDefault(x => x.FlatId == f.FlatId);
                return new FavoriteResponse(
                        f.FavoriteId,
                        f.FlatId,
                        flatInfo ?? throw new Exception($"Flat info not found for ID {f.FlatId}")
                );
            }).ToList();

            return new PagedResponse<FavoriteResponse>(result, favorites.Count, page, pageSize);
        }
        
        
        
        
        public async Task AddComparisonAsync(Guid userId, AddComparisonRequest request)
        {
            await _usersRepo.GetById(userId); // Проверка существования пользователя

            if (request.FlatId1 == request.FlatId2)
                throw new ArgumentException("Cannot compare flat with itself");

            var flat1Exists = await _flatsRepo.GetFlatById(request.FlatId1) != null;
            var flat2Exists = await _flatsRepo.GetFlatById(request.FlatId2) != null;
            
            if (!flat1Exists || !flat2Exists)
                throw new ArgumentException("One or both flats not found");

            if (await _userPreferencesRepo.ComparisonExistsAsync(userId, request.FlatId1, request.FlatId2))
                throw new InvalidOperationException("Comparison already exists");

            await _userPreferencesRepo.AddComparisonAsync(userId, request.FlatId1, request.FlatId2);
        }

        public async Task RemoveComparisonAsync(Guid userId, int compareId)
        {
            var comparison = (await _userPreferencesRepo.GetUserComparisonsAsync(userId))
                .FirstOrDefault(c => c.CompareId == compareId);

            if (comparison == null)
                throw new UnauthorizedAccessException("Flat is not in favorites of user");

            await _userPreferencesRepo.RemoveComparisonAsync(compareId);
        }

        public async Task<List<ComparisonResponse>> GetUserComparisonsAsync(Guid userId)
        {
            var comparisons = await _userPreferencesRepo.GetUserComparisonsAsync(userId);
            if (!comparisons.Any()) 
                return new List<ComparisonResponse>();

            var flatIds = comparisons
                .SelectMany(c => new[] { c.FlatId1, c.FlatId2 })
                .Distinct()
                .ToList();

            var flatsInfo = await _flatsRepo.GetFlatsWithDetails(flatIds);

            return comparisons.Select(c => 
            {
                var flat1 = flatsInfo.FirstOrDefault(f => f.FlatId == c.FlatId1);
                var flat2 = flatsInfo.FirstOrDefault(f => f.FlatId == c.FlatId2);

                if (flat1 == null || flat2 == null)
                    throw new Exception($"One or both flats not found for comparison {c.CompareId}");

                return new ComparisonResponse(
                    c.CompareId,
                    flat1,
                    flat2);
            }).ToList();
        }
        
        public async Task<PagedResponse<ComparisonResponse>>GetPagedUserComparisonsAsync(Guid userId, int page, int pageSize)
        {
            var comparisons = await _userPreferencesRepo.GetUserComparisonsAsync(userId);
           
            var paginatedComparisons = comparisons
                .OrderByDescending(c => c.CompareId)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            
            var flatIds = comparisons
                .SelectMany(c => new[] { c.FlatId1, c.FlatId2 })
                .Distinct()
                .ToList();
            var flatsInfo = await _flatsRepo.GetFlatsWithDetails(flatIds);

            var result = paginatedComparisons.Select(c =>
            {
                var flat1 = flatsInfo.FirstOrDefault(f => f.FlatId == c.FlatId1);
                var flat2 = flatsInfo.FirstOrDefault(f => f.FlatId == c.FlatId2);

                if (flat1 == null || flat2 == null)
                    throw new Exception($"One or both flats not found for comparison {c.CompareId}");

                return new ComparisonResponse(
                    c.CompareId,
                    flat1,
                    flat2);
            }).ToList();
            
            return new PagedResponse<ComparisonResponse>(result, comparisons.Count, page, pageSize);
        }
    }
}