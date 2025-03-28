namespace Entities;

public class UserFavorite
{
    public int FavoriteId { get; set; }
    public int UserId { get; set; }
    public int FlatId { get; set; }
    
    public User User { get; set; }
    public Flat Flat { get; set; }
}