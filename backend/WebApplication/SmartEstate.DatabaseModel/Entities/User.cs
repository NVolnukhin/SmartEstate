namespace DatabaseModel;

public class User
{
    public Guid UserId { get; set; }
    public string Login { get; set; } = string.Empty;
    public string HashedPassword { get; set; } = string.Empty;
    public string Email { get; set; }
    public string? Name { get; set; } = string.Empty;
    
    public static User Create(Guid id, string login, string email, string passwordHash, string name)
    {
        if (string.IsNullOrWhiteSpace(login))
            throw new ArgumentException("Логин не может быть пустым", nameof(login));
        
        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email не может быть пустым", nameof(email));

        return new User
        {
            UserId = id,
            Login = login.Trim(),
            Email = email.Trim().ToLower(),
            HashedPassword = passwordHash ?? throw new ArgumentNullException(nameof(passwordHash)),
            Name = name?.Trim()
        };
    }
    
    public ICollection<UserComparison> Comparisons { get; set; } = new List<UserComparison>();
    public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
}