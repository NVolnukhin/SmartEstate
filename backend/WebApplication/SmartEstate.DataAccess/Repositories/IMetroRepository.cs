using DatabaseModel.Infrastucture;

namespace SmartEstate.DataAccess.Repositories;

public interface IMetroRepository
{
    Task<List<string>> GetAllMetroStationsAsync();
}
