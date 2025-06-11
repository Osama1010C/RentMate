using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Helpers
{
    public static class RentalHelper
    {
        public static async Task<bool> IsAskedForRentAsync(IUnitOfWork unitOfWork, int tenantId, int propertyId)
        {
            var rentingProperty = await unitOfWork.RentalRequests
                        .GetAllAsync(r => r.TenantId == tenantId && r.PropertyId == propertyId && r.Status == "pending");

            return rentingProperty.Count() > 0;
        }

        public static async Task<bool> IsRequestedBeforeAsync(IUnitOfWork unitOfWork, int tenantId, int propertyId)
        {
            var request = await unitOfWork.RentalRequests
                        .GetAllAsync(r => r.TenantId == tenantId && r.PropertyId == propertyId && (r.Status == "rejected" || r.Status == "pending"));

            return request.Count() > 0;
        }
    }
}
