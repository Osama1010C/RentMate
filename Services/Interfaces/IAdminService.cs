using RentMateAPI.DTOModels.DTODashboard;

namespace RentMateAPI.Services.Interfaces
{
    public interface IAdminService
    {
        Task AcceptPropertyPostAsync(int propertyId);
        Task AcceptLandlordRegistrationAsync(int requestId);

        Task RejectPropertyPostAsync(int propertyId);
        Task RejectLandlordRegistrationAsync(int requestId);

        Task<DashboardDto> GetStatisticsAsync();
    }
}
