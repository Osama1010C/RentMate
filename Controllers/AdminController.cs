using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin")]
    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            this._adminService = adminService;
        }


        /// <summary>Return statistics of system</summary>
        [HttpGet("DashBoard")]
        public async Task<IActionResult> GetStatistics()
            => Ok(await _adminService.GetStatisticsAsync());



        /// <summary>Allow admin to accept request of landlord to register in system</summary>
        [HttpPost("AcceptRegistration/{requestId}")]
        public async Task<IActionResult> AcceptLandlordRegistration(int requestId)
        {
            try
            {
                await _adminService.AcceptLandlordRegistrationAsync(requestId);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }



        /// <summary>Allow admin to accept request of landlord to add his property in system</summary>
        [HttpPost("AcceptPost/{propertyId}")]
        public async Task<IActionResult> AcceptPropertPost(int propertyId)
        {
            try
            {
                await _adminService.AcceptPropertyPostAsync(propertyId);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }



        /// <summary>Allow admin to reject request of landlord to register in system</summary>
        [HttpDelete("LandlordRequest/{requestId}")]
        public async Task<IActionResult> RejectLandlordRegistration(int requestId)
        {
            try
            {
                await _adminService.RejectLandlordRegistrationAsync(requestId);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }


        /// <summary>Allow admin to reject request of landlord to add his property in system</summary>
        [HttpDelete("RejectPost/{propertyId}")]
        public async Task<IActionResult> RejectPropertyPost(int propertyId)
        {
            try
            {
                await _adminService.RejectPropertyPostAsync(propertyId);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }

        

    }
}
