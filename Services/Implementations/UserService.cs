using RentMateAPI.DTOModels.DTODashboard;
using RentMateAPI.DTOModels.DTOUser;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using System.Data;

namespace RentMateAPI.Services.Implementations
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        public UserService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
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
            var user = await _unitOfWork.Users.GetByIdAsync(id);

            if (user is null) throw new Exception($"user with {id} not exist");

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
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) throw new Exception($"user with {userId} not exist");
            return user.Image!;
                
        }
        public async Task AddImageAsync(UserImageDto userImage)
        {
            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
            var file = userImage.Image;
            var extension = Path.GetExtension(file.FileName);

            if(!IsValidFileExtension(extension, allowedExtensions))
                throw new Exception($"Invalid file extension: {extension}. Allowed extensions are: {string.Join(", ", allowedExtensions)}");

            if (!IsValidFileSize(file.Length, 1 * 1024 * 1024))
                throw new Exception("File size exceeds the maximum limit of 1MB.");


            var user = await _unitOfWork.Users.GetByIdAsync(userImage.Id);
            if (user is null) throw new Exception($"user with {userImage.Id} not exist");

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

        private bool IsValidFileExtension(string extension, List<string> allowedExtensions)
            => allowedExtensions.Contains(extension.ToLower());
        private bool IsValidFileSize(long fileSize, long allowedSize)
            => fileSize <= allowedSize;


    }
}
