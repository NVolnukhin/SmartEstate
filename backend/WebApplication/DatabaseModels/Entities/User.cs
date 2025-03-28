namespace Entities;

public class User
{
    public int UserId { get; set; }
    public string Login { get; set; }
    public string HashedPassword { get; set; }
    public string? Email { get; set; }
    public string Name { get; set; }
    
    public ICollection<UserComparison> Comparisons { get; set; } = new List<UserComparison>();
    public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
}