using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    [Authorize(Roles = "landlord")]
    public class LandlordController : ControllerBase
    {
        private readonly IRentalService _rentalService;

        public LandlordController(IRentalService rentalService)
        {
            this._rentalService = rentalService;
        }

        

        [HttpPost("AcceptRentRequest/{rentId}")]
        public async Task<IActionResult> AcceptRentRequest(int rentId)
        {
            try
            {
                await _rentalService.AcceptRequestAsync(rentId);
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }

        [HttpPost("RejectRentRequest/{rentId}")]
        public async Task<IActionResult> RejectRentRequest(int rentId)
        {
            try
            {
                await _rentalService.RejectRequestAsync(rentId);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }

            return Ok();
        }

        
    }
}
