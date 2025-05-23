using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOToken;

namespace RentMateAPI.Services.Interfaces
{
    public interface IAuthService
    {
        Task<List<User>> GetAllAsync();

        Task<AuthModelDto> RegistAsync(User tenant);

        Task<AuthModelDto> LoginAsync(string name, string password);


    }
}
