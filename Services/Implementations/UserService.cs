using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOUser;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using RentMateAPI.Validations.Interfaces;
using System.Data;

namespace RentMateAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageValidator _imageValidator;
        private readonly IModelValidator<User> _userValidator;
        public UserService(IUnitOfWork unitOfWork, IImageValidator imageValidator, IModelValidator<User> userValidator)
        {
            _unitOfWork = unitOfWork;
            _imageValidator = imageValidator;
            _userValidator = userValidator;
        }


        public async Task<List<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var dtoUsers = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Image = user.Image,
                Role = user.Role
            }).ToList();

            return dtoUsers;
        }

        public async Task<UserDto> GetUserAsync(int id)
        {
            var user = await _userValidator.IsModelExistReturn(id);

            var dtoUser = new UserDto
            {
                Id = id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Image = user.Image,
                Role = user.Role
            };
            return dtoUser;
        }

        public async Task<StatisticsDto> GetStatisticsAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var numOfUsers = users.Count();
            var numOfTenants = users.Count(u => u.Role == "tenant");
            var numOfLandlords = users.Count(u => u.Role == "landlord");

            var properties = await _unitOfWork.Properties.GetAllAsync();
            var numOfProperties = properties.Count(p => p.PropertyApproval == "accepted");


            var statistics = new StatisticsDto
            {
                NumberOfUsers = numOfUsers,
                NumberOfTenants = numOfTenants,
                NumberOfLandlords = numOfLandlords,
                NumberOfProperties = numOfProperties,
            };

            return statistics;
        }

        public async Task<byte[]> GetUserImageAsync(int userId)
        {
            var user = await _userValidator.IsModelExistReturn(userId);
            return user.Image!;
        }
        public async Task AddImageAsync(UserImageDto userImage)
        {
            _imageValidator.IsValidImageExtension(userImage.Image);
            _imageValidator.IsValidImageSize(userImage.Image);

            var user = await _userValidator.IsModelExistReturn(userImage.Id);

            byte[]? mainImageBytes = null;
            using (var memoryStream = new MemoryStream())
            {
                await userImage.Image!.CopyToAsync(memoryStream);
                mainImageBytes = memoryStream.ToArray();
            }
            user = await _unitOfWork.Users.GetByIdAsync(userImage.Id);
            user!.Image = mainImageBytes;
            await _unitOfWork.CompleteAsync();
        }
    }
}
