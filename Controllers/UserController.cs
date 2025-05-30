﻿using Microsoft.AspNetCore.Authorization;
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
        [Authorize(Roles = "admin,landlord,tenant")]
        public async Task<IActionResult> GetAllUsers() => Ok(await _userService.GetAllUsersAsync());


        [HttpGet("{id}")]
        [Authorize(Roles = "admin,landlord,tenant")]
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



        [HttpGet("Statistics")]
        public async Task<IActionResult> GetStatistics()
        {
            try
            {
                var statistics = await _userService.GetStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        [HttpGet("{id}/image")]
        [Authorize(Roles = "admin,landlord,tenant")]
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
        [Authorize(Roles = "admin,landlord,tenant")]
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
    }
}
