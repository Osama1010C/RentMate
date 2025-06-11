using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOToken;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using System.IdentityModel.Tokens.Jwt;

namespace RentMateAPI.Services.Implementations
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly JwtService _jwtService;
        public AuthService(IUnitOfWork unitOfWork, JwtService jwtService)
        {
            _jwtService = jwtService;
            _unitOfWork = unitOfWork;
        }


        public async Task<AuthModelDto> RegistAsync(User tenant)
        {
            bool isAlreadyExist = false;
            bool isAlreadyExistInPendings = false;
            
            var hashedPassword = BC.EnhancedHashPassword(tenant.Password);

            var users = await _unitOfWork.Users.GetAllAsync();
            var pendings = await _unitOfWork.PendingLandlord.GetAllAsync();


            isAlreadyExist = users.Any(u => u.Name == tenant.Name || u.Email == tenant.Email || BC.EnhancedVerify(tenant.Password, u.Password));
            isAlreadyExistInPendings = pendings.Any(p => p.Name == tenant.Name || p.Email == tenant.Email || BC.EnhancedVerify(tenant.Password, p.Password));

            if (isAlreadyExist || isAlreadyExistInPendings) throw new Exception("Name or Email or password already exists."); //Conflict("Nmae or Email or password already exists.");


            var newUser = new User
            {
                Name = tenant.Name,
                Email = tenant.Email,
                Password = hashedPassword,
                Role = "tenant"
            };

            await _unitOfWork.Users.AddAsync(newUser);
            await _unitOfWork.CompleteAsync();

            var token = _jwtService.GenerateToken(newUser);
            var ExpiresOn = token.ValidTo;

            var Token = new JwtSecurityTokenHandler().WriteToken(token);
            
            var refreshToken = Guid.NewGuid().ToString();

            
            return new AuthModelDto
            {
                Id = newUser.Id,
                Token = Token,
                RefreshToken = refreshToken,
                ExpiresOn = ExpiresOn,
                IsAuthenticated = true
            };

        }

        public async Task<AuthModelDto> LoginAsync(string name, string password)
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            var isAuthorized = users.Any(u => u.Name == name && BC.EnhancedVerify(password, u.Password));

            if (!isAuthorized) throw new Exception("Name or password is wrong");

            var user = users.FirstOrDefault(u => u.Name == name && BC.EnhancedVerify(password, u.Password));

            var token = _jwtService.GenerateToken(user!);
            var ExpiresOn = token.ValidTo;

            var Token = new JwtSecurityTokenHandler().WriteToken(token);

            var refreshToken = Guid.NewGuid().ToString();

            return new AuthModelDto
            {
                Id = user!.Id,
                Token = Token,
                RefreshToken = refreshToken,
                ExpiresOn = ExpiresOn,
                IsAuthenticated = true
            };
        }
    }
}
