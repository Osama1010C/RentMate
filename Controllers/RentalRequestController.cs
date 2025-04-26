using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RentMateAPI.Data;
using RentMateAPI.DTOModels.DTORent;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    public class RentalRequestController : ControllerBase
    { 
        private readonly IRentalService _rentalService;
        public RentalRequestController(IRentalService rentalService)
        {
            this._rentalService = rentalService;
        }


        [HttpGet("Requests/{landlordId}")]
        //[Authorize(Roles = "landlord")]
        public async Task<IActionResult> GetAllRentingRequests(int landlordId)
        {
            try
            {
                return Ok(await _rentalService.GetAllRequestsAsync(landlordId));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("MyRentRequests/{tenantId}")]
        //[Authorize(Roles = "tenant")]
        public async Task<IActionResult> GetAllTenantRentRequests(int tenantId)
        {
            try
            {
                return Ok(await _rentalService.GetTenantRequestsAsync(tenantId));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpPost]
        //[Authorize(Roles = "tenant")]
        public async Task<IActionResult> RentProperty([FromForm] RentPropertyDto rentDto)
        {
            try
            {
                await _rentalService.RentPropertyAsync(rentDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
