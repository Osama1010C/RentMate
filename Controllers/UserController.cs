// THIS CONTROLLER IS JUST FOR TESTING //
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.DTOModels.DTOUser;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            this._userService = userService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers() => Ok(await _userService.GetAllUsersAsync());


        [HttpGet("{id}")]
        public async Task<IActionResult> GetUser(int id)
        {
            try
            {
                var dtoUser = await _userService.GetUserAsync(id);
                return Ok(dtoUser);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpGet("{id}/image")]
        public async Task<IActionResult> GetUserImage(int id)
        {
            try
            {
                var image = await _userService.GetUserImageAsync(id);
                return Ok(new { image = image });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
        [HttpPost("image")]
        public async Task<IActionResult> AddUserImage([FromForm]UserImageDto user)
        {
            try
            {
                await _userService.AddImageAsync(user);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddUser([FromBody] NewUserDto userDto, string role)
            => Ok(await _userService.AddUserAsync(userDto, role));

    }
}
