using DatabaseModel;

namespace SmartEstate.DataAccess.Repositories;

public interface IFlatsRepository
{
    public Task<List<Flat>> GetAllFlats();
    public Task<List<Flat>> GetRandomFlats(int count);
    public Task<Flat?> GetFlatById(int flatId);
    public Task<IEnumerable<PriceHistory>> GetPriceHistory();
    public Task<IEnumerable<PriceHistory>> GetLatestPrices();
}