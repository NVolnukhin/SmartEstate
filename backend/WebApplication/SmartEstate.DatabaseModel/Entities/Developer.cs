using System.Collections.Generic;

namespace DatabaseModel;

public class Developer
{
    public int DeveloperId { get; set; }
    public string? Website { get; set; }
    public string? BuildingsCount { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public ICollection<Building> Buildings { get; set; } = new List<Building>();
}