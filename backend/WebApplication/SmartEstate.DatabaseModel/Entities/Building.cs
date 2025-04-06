namespace DatabaseModel;

public class Building
{
    public int BuildingId { get; set; }
    public int DeveloperId { get; set; }
    public string ConstructionStatus { get; set; } = string.Empty;
    public int FloorCount { get; set; }
    public string Address { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
    public string ResidentialComplex { get; set; }
    
    public Developer Developer { get; set; }
    public ICollection<Flat> Flats { get; set; } = new List<Flat>();
}