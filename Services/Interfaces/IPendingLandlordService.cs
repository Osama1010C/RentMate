using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOUser;

namespace RentMateAPI.Services.Interfaces
{
    public interface IPendingLandlordService
    {
        Task<List<PendingLandlordsDto>> GetAllAsync();

        Task<bool> AddAsync(NewUserDto landlord);
    }
}
