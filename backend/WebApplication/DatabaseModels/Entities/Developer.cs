namespace Entities;

public class Developer
{
    public int DeveloperId { get; set; }
    public string? Website { get; set; }
    public int BuildingsCount { get; set; }
    public string Name { get; set; }
    
    public ICollection<Building> Buildings { get; set; } = new List<Building>();
}