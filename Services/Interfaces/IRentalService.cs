using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.DTOModels.DTORent;

namespace RentMateAPI.Services.Interfaces
{
    public interface IRentalService
    {
        Task AcceptRequestAsync(int requestId);
        Task RejectRequestAsync(int requestId); 

        Task<List<RentPropertyRequestDto>> GetAllRequestsAsync(int landlordId);
        Task<List<RentPropertyRequestDto>> GetAllRequestsAsync(int landlordId, int propertyId);
        Task<List<PropertyRequestDto>> GetTenantRequestsAsync(int tenantId);

        Task RentPropertyAsync(RentPropertyDto rentDto);
        Task CancelRentPropertyAsync(int tenantId, int propertyId);
    }
}
