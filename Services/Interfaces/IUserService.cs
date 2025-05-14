using RentMateAPI.DTOModels.DTOUser;

namespace RentMateAPI.Services.Interfaces
{
    public interface IUserService
    {
        Task<List<UserDto>> GetAllUsersAsync();

        Task<UserDto> GetUserAsync(int id);

        Task<byte[]> GetUserImageAsync(int userId);
        Task AddImageAsync(UserImageDto userImage);
    }
}
