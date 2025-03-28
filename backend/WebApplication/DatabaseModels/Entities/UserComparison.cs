namespace Entities;

public class UserComparison
{
    public int CompareId { get; set; }
    public int UserId { get; set; }
    public int FlatId1 { get; set; }
    public int FlatId2 { get; set; }
    
    public User User { get; set; }
    public Flat FirstFlat { get; set; }
    public Flat SecondFlat { get; set; }
}