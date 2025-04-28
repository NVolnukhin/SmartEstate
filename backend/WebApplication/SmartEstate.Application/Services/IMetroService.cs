using Contracts.InfrastructureInfo;
using DatabaseModel.Infrastucture;

namespace SmartEstate.Application.Services;

public interface IMetroService
{
    Task<List<MetroDto>> GetAllMetroStationsAsync();
}