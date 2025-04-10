using DatabaseModel.Infrastucture;

namespace SmartEstate.Application.Services;

public interface IMetroService
{
    Task<List<string>> GetAllMetroStationsAsync();
}