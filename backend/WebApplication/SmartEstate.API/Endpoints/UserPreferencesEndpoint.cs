using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Presentation.Contracts.Comparisons;
using Presentation.Contracts.Favorites;
using SmartEstate.ApplicationServices;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/user-preferences")]
    public class UserPreferencesEndpoint : ControllerBase
    {
        private readonly IUserPreferencesService _userPreferencesService;

        public UserPreferencesEndpoint(IUserPreferencesService userPreferencesService)
        {
            _userPreferencesService = userPreferencesService;
        }
        
        [HttpPost("favorites")]
        public async Task<IActionResult> AddFavorite([FromBody] AddFavoriteRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                Console.WriteLine($"UserID = {userId}\n adding favorite");
                await _userPreferencesService.AddFavoriteAsync(userId, request);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }


        [HttpDelete("favorites/{favoriteId}")]
        public async Task<IActionResult> RemoveFavorite(int favoriteId)
        {
            try
            {
                var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                Console.WriteLine($"UserID = {userId}\n deleting fav {favoriteId}");
                await _userPreferencesService.RemoveFavoriteAsync(userId, favoriteId);
                
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet("favorites")]
        public async Task<ActionResult<List<FavoriteResponse>>> GetFavorites()
        {
            try
            {
                var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                var favorites = await _userPreferencesService.GetUserFavoritesAsync(userId);
                return Ok(favorites);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpPost("comparisons")]
        public async Task<IActionResult> AddComparison([FromBody] AddComparisonRequest request)
        {
            try
            {
                var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                Console.WriteLine($"UserID = {userId}\n     adding compare {request.FlatId1} with {request.FlatId2}");
                await _userPreferencesService.AddComparisonAsync(userId, request);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpDelete("comparisons/{comparisonId}")]
        public async Task<IActionResult> RemoveComparison(int comparisonId)
        {
            try
            {
                var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                Console.WriteLine($"UserID = {userId}\n     deleting compare {comparisonId}");
                await _userPreferencesService.RemoveComparisonAsync(userId, comparisonId);
                return Ok();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet("comparisons")]
        public async Task<ActionResult<List<ComparisonResponse>>> GetComparisons()
        {
            try
            {
                var userId = Guid.Parse(User.Claims.First(c => c.Type == ClaimTypes.NameIdentifier).Value);
                var comparisons = await _userPreferencesService.GetUserComparisonsAsync(userId);
                return Ok(comparisons);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }
    }
}