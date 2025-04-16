using Contracts.InfrastructureInfo;
using DatabaseModel.Infrastucture;
using SmartEstate.DataAccess.Repositories;

namespace SmartEstate.Application.Services;

public class MetroService : IMetroService
{
    private readonly IMetroRepository _metroRepository;
    
    public MetroService(IMetroRepository metroRepository)
    {
        _metroRepository = metroRepository;
    }

    public async Task<List<MetroDto>> GetAllMetroStationsAsync()
    {
        return await _metroRepository.GetAllMetroStationsAsync();
    }
}