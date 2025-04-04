using DatabaseModel.Infrastucture;

namespace DatabaseModel;


public class InfrastructureInfo
{
    public int? InfrastructureInfoId { get; set; }
    public int BuildingId { get; set; }
    public string? Facilities { get; set; } = string.Empty;
    
    // Navigation properties to nearest infrastructure
    public int? NearestShopId { get; set; }
    public int? NearestMetroId { get; set; }
    public int? NearestSchoolId { get; set; }
    public int? NearestKindergartenId { get; set; }
    public int? NearestPharmacyId { get; set; }
    
    public int? MinutesToShop { get; set; }
    public int? MinutesToMetro { get; set; }
    public int? MinutesToSchool { get; set; }
    public int? MinutesToKindergarten { get; set; }
    public int? MinutesToPharmacy { get; set; }
    
    // Navigation properties
    public Building? Building { get; set; }
    public Shop? NearestShop { get; set; }
    public Metro? NearestMetro { get; set; }
    public School? NearestSchool { get; set; }
    public Kindergarten? NearestKindergarten { get; set; }
    public Pharmacy? NearestPharmacy { get; set; }
}
