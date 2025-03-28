namespace DatabaseModel;

public class PriceHistory
{
    public int PriceHistoryId { get; set; }
    public int FlatId { get; set; }
    public decimal Price { get; set; }
    public DateTime ChangeDate { get; set; }
    
    public Flat Flat { get; set; }
}