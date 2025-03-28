using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DatabaseModel;

public class UserComparison
{
    [Key]
    public int CompareId { get; set; }
    
    [ForeignKey("User")]
    public int UserId { get; set; }
    
    [ForeignKey("FirstFlat")]
    public int FlatId1 { get; set; }
    
    [ForeignKey("SecondFlat")]
    public int FlatId2 { get; set; }
    
    public User User { get; set; }
    public Flat FirstFlat { get; set; }
    public Flat SecondFlat { get; set; }
}