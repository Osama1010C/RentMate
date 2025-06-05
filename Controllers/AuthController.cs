using Microsoft.AspNetCore.Mvc;
using RentMateAPI.DTOModels.DTOUser;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOToken;
using RentMateAPI.Services.Implementations;
using RentMateAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authentication;


namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController] 
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly IPendingLandlordService _pendingLandlordService;
        private readonly IAuthService _authService;
        public AuthController(JwtService _jwtService, 
                                      IPendingLandlordService pendingLandlordService, IAuthService authService)
        {
            this._jwtService = _jwtService;
            this._pendingLandlordService = pendingLandlordService;
            _authService = authService;
        }


        
        [HttpGet("LandlordsRequests")]
        public async Task<IActionResult> GetLandlordRegistrationRequests() => Ok(await _pendingLandlordService.GetAllAsync());

     

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] NewUserDto userDto, string role)
        {
            // landlord
            if (role.ToLower() == "landlord")
            {
                if(!await _pendingLandlordService.AddAsync(new()
                {
                    Name = userDto.Name,
                    Email = userDto.Email,
                    Password = userDto.Password,
                }))
                {
                    return Conflict("Name or Email or password already exists.");
                }
            }

            // tenant
            else
            {
                AuthModelDto? authModel = null;
                try
                {
                    authModel = await _authService.RegistAsync(new User
                    {
                        Name = userDto.Name,
                        Email = userDto.Email,
                        Password = userDto.Password,
                        Role = "tenant"
                    });

                    HttpContext.Session.SetString("refreshToken:" + authModel.Id, authModel.RefreshToken);
                    return Ok(authModel);
                }
                catch (Exception ex)
                {
                    return Conflict(ex.Message);
                }
            }


            return Ok();
        }

        

        [HttpPost("Login")]
        public async Task<IActionResult> Login(string name, string password)
        {
            AuthModelDto? authModel = null;
            try
            {
                authModel = await _authService.LoginAsync(name, password);

                HttpContext.Session.SetString("refreshToken:" + authModel.Id, authModel.RefreshToken);
                return Ok(authModel);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }

    

        

        [HttpPost("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto model)
        {
            var sessionKey = "refreshToken:" + model.UserId;
            var storedToken = HttpContext.Session.GetString(sessionKey);
            var newAuthModel = new AuthModelDto();

            try
            {
                newAuthModel = await _jwtService.RefreshTokenAsync(model.UserId, storedToken!, model.RefreshToken);
                HttpContext.Session.SetString(sessionKey, newAuthModel.RefreshToken);
            }
            catch (Exception ex)
            {
                //HttpContext.Session.Clear();
                //Response.Cookies.Delete(".AspNetCore.Session");
                return Ok(new { message = "Token NOT Refreshed." });
            }


            return Ok(newAuthModel);
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");

            return Ok(new { message = "Logged out successfully." });
        }

    }
}
