using RentMateAPI.Data.Models;

namespace RentMateAPI.Services.Interfaces
{
    public interface IPendingLandlordService
    {
        Task<List<PendingLandlord>> GetAllAsync();

        Task<bool> AddAsync(PendingLandlord landlord);
    }
}
