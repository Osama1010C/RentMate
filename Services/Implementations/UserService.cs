using Microsoft.AspNetCore.Mvc;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOProperty;
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

        

        public async Task<UserDto> AddUserAsync(NewUserDto userDto, string role)
        {
            var hashedPassword = BC.EnhancedHashPassword(userDto.Password);

            var user = new User()
            {
                Name = userDto.Name,
                Email = userDto.Email,
                Password = hashedPassword,
                Role = role
            };

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.CompleteAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Password = user.Password,
                Image = user.Image,
                Role = user.Role
            };
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

        public async Task<byte[]> GetUserImageAsync(int userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user is null) throw new Exception($"user with {userId} not exist");
            return user.Image!;
                
        }
        public async Task AddImageAsync(UserImageDto userImage)
        {
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
    }
}
