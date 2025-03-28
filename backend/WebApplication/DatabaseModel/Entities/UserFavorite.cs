using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseModel;

public class UserFavorite
{
    [Key]
    public int FavoriteId { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [ForeignKey("Flat")]
    public int FlatId { get; set; }
    
    public User User { get; set; }
    public Flat Flat { get; set; }
}