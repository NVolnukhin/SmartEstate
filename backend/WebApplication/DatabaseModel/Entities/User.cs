namespace DatabaseModel;

public class User
{
    public int UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ICollection<UserComparison> Comparisons { get; set; } = new List<UserComparison>();
    public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
}