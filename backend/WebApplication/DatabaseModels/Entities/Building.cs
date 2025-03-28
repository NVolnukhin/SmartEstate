namespace Entities;

public class Building
{
    public int BuildingId { get; set; }
    public int DeveloperId { get; set; }
    public string ConstructionStatus { get; set; }
    public int FloorCount { get; set; }
    public string Address { get; set; }
    public int? InfrastructureInfoId { get; set; }
    
    public Developer Developer { get; set; }
    public InfrastructureInfo? InfrastructureInfo { get; set; }
    public ICollection<Flat> Flats { get; set; } = new List<Flat>();
}