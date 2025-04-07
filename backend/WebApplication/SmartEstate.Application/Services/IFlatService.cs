using System.Collections.Generic;
using System.Threading.Tasks;
using Contracts;
using Contracts.Flats;
using Presentation.Contracts.Flats;

namespace SmartEstate.Application.Services;

public interface IFlatService
{
    Task<PagedResponse<FlatResponse>> GetAllFlatsAsync(int page, int pageSize);
    Task<List<FlatShortInfoResponse>> GetRandomFlatsAsync(int count);
    Task<FlatDetailsResponse?> GetFlatDetailsByIdAsync(int flatId);
}