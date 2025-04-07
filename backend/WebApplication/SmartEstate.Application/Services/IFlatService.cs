using System.Collections.Generic;
using System.Threading.Tasks;
using Presentation.Contracts.Flats;

namespace SmartEstate.Application.Services;

public interface IFlatService
{
    Task<List<FlatResponse>> GetAllFlatsAsync();
    Task<List<FlatShortInfoResponse>> GetRandomFlatsAsync();
    Task<FlatDetailsResponse?> GetFlatDetailsByIdAsync(int flatId);
}