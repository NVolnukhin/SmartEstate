namespace DatabaseModel;

public class Flat
{
    public int FlatId { get; set; }
    public string? Images { get; set; } //  picture links
    public decimal Square { get; set; }
    public int Roominess { get; set; }
    public int Floor { get; set; }
    public string CianLink { get; set; }  = string.Empty; // Link fot flat on cian 
    public int BuildingId { get; set; }
    public string FinishType { get; set; } = string.Empty;
    
    public Building Building { get; set; }
    public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
    public ICollection<UserComparison> ComparisonsAsFirst { get; set; } = new List<UserComparison>();
    public ICollection<UserComparison> ComparisonsAsSecond { get; set; } = new List<UserComparison>();
    public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
}