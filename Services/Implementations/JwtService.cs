using Microsoft.IdentityModel.Tokens;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOToken;
using RentMateAPI.Repositories.Interfaces;
using RentMateAPI.UOF.Interface;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RentMateAPI.Services.Implementations
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        //private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;


        public JwtService(IConfiguration configuration, IUnitOfWork unitOfWork)
        {
            _configuration = configuration;
            //_userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }

        public JwtSecurityToken GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JWT:DurationInMinutes"])),
                signingCredentials: creds);

            return token;
        }

        public async Task<AuthModelDto> RefreshTokenAsync(int userId, string storedToken, string refreshToken)
        {

            if (storedToken == null || storedToken != refreshToken)
                throw new Exception("Session expired or refresh token invalid");

            //var user = await _userRepository.GetByIdAsync(userId);
            var user = await _unitOfWork.Users.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            var newToken = GenerateToken(user);
            var newRefreshToken = Guid.NewGuid().ToString();


            var authModel = new AuthModelDto
            {
                Id = user.Id,
                IsAuthenticated = true,
                Token = new JwtSecurityTokenHandler().WriteToken(newToken),
                ExpiresOn = newToken.ValidTo,
                RefreshToken = newRefreshToken
            };

            return authModel;
        }


        
    }
}
