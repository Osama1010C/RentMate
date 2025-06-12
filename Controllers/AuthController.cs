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



        /// <summary>Return all landlords registration requests to admin</summary>
        [HttpGet("LandlordsRequests")]
        public async Task<IActionResult> GetLandlordRegistrationRequests() => Ok(await _pendingLandlordService.GetAllAsync());



        /// <summary>Register new user , return token if user is tenant , return nothing of user is landlord and move him to pendings accounts</summary>
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



        /// <summary>Login user and return token</summary>
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody]UserLoginDto userDto)
        {
            AuthModelDto? authModel = null;
            try
            {
                authModel = await _authService.LoginAsync(userDto.Name, userDto.Password);

                HttpContext.Session.SetString("refreshToken:" + authModel.Id, authModel.RefreshToken);
                return Ok(authModel);
            }
            catch (Exception ex)
            {
                return Conflict(ex.Message);
            }

        }





        /// <summary>Refresh old token and return new token</summary>
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
                return Ok(new { message = "Token NOT Refreshed." });
            }


            return Ok(newAuthModel);
        }


        /// <summary>Logout user and clear session</summary>
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            HttpContext.Session.Clear();
            Response.Cookies.Delete(".AspNetCore.Session");

            return Ok(new { message = "Logged out successfully." });
        }

    }
}
