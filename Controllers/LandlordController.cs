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



        /// <summary>Allow landlord to accept request for renting his property</summary>
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


        /// <summary>Allow landlord to reject request for renting his property</summary>
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
