namespace Entities;

public class InfrastructureInfo
{
    public int? InfrastructureInfoId { get; set; }
    public int? MinutesToShop { get; set; }
    public int? MinutesToMetro { get; set; }
    public int? MinutesToSchool { get; set; }
    public int? MinutesToKindergarten { get; set; }
    public int? MinutesToBusStop { get; set; }
    public int? MinutesToPharmacy { get; set; }
    public string? Facilities { get; set; }
    
    public Building? Building { get; set; }
}