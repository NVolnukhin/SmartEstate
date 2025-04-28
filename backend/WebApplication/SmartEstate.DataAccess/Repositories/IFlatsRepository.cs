using DatabaseModel;
using Presentation.Contracts.Flats;

namespace SmartEstate.DataAccess.Repositories;

public interface IFlatsRepository
{
    public Task<List<Flat>> GetAllFlats();
    public Task<List<Flat>> GetRandomFlats(int count);
    public Task<Flat?> GetFlatById(int flatId);
    public Task<IEnumerable<PriceHistory>> GetPriceHistory();
    public Task<IEnumerable<PriceHistory>> GetLatestPrices();
    Task<List<FlatShortInfoResponse>> GetFlatsWithDetails(List<int> flatIds);
}