using RentMateAPI.DTOModels.DTORent;

namespace RentMateAPI.Services.Interfaces
{
    public interface IRentalService
    {
        Task AcceptRequestAsync(int requestId);
        Task RejectRequestAsync(int requestId); 

        Task<List<RentPropertyRequestDto>> GetAllRequestsAsync(int landlordId);
        Task<List<TenantRentRequestDto>> GetTenantRequestsAsync(int tenantId);

        Task RentPropertyAsync(RentPropertyDto rentDto);
    }
}
