using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace DatabaseModel;

[Table("Flats")]
public class Flat
{
    public int FlatId { get; set; }

    [Column("Images")]
    public string ImagesJson { get; set; }

    [NotMapped]
    public string[]? Images
    {
        get => JsonConvert.DeserializeObject<string[]>(ImagesJson) ?? Array.Empty<string>();
        set => ImagesJson = JsonConvert.SerializeObject(value);
    }

    public decimal Square { get; set; }
    public int Roominess { get; set; }
    public int Floor { get; set; }
    public string CianLink { get; set; }  = string.Empty;  
    public int BuildingId { get; set; }
    public string FinishType { get; set; } = string.Empty;
    
    public Building Building { get; set; }
    public ICollection<PriceHistory> PriceHistories { get; set; } = new List<PriceHistory>();
    public ICollection<UserComparison> ComparisonsAsFirst { get; set; } = new List<UserComparison>();
    public ICollection<UserComparison> ComparisonsAsSecond { get; set; } = new List<UserComparison>();
    public ICollection<UserFavorite> Favorites { get; set; } = new List<UserFavorite>();
}