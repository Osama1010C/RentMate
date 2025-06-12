using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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



        /// <summary>Return requested properties for rent for a specific landlord</summary>
        [HttpGet("Requests/{landlordId}")]
        [Authorize(Roles = "landlord")]
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


        /// <summary>Return requested property for rent for a specific landlord</summary>
        [HttpGet("Requests/{landlordId}/{propertyId}")]
        [Authorize(Roles = "landlord")]
        public async Task<IActionResult> GetAllRentingRequestsForProperty(int landlordId, int propertyId)
        {
            try
            {
                return Ok(await _rentalService.GetAllRequestsAsync(landlordId, propertyId));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>Return all tenant's requests for renting properties</summary>
        [HttpGet("MyRentRequests/{tenantId}")]
        [Authorize(Roles = "tenant")]
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


        /// <summary>Send rent request for a property</summary>
        [HttpPost]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> RentProperty(int tenantId, int propertyId, IFormFile document)
        {
            try
            {
                await _rentalService.RentPropertyAsync(new RentPropertyDto
                {
                    TenantId = tenantId,
                    PropertyId = propertyId,
                    RequirmentDocument = document
                });
                return Ok();
            }
            catch (InvalidDataException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>Allow tenant to cancel his rent request</summary>
        [HttpDelete]
        [Authorize(Roles = "tenant")]
        public async Task<IActionResult> CancelRentRequestProperty(int tenantId, int propertyId)
        {
            try
            {
                await _rentalService.CancelRentPropertyAsync(tenantId, propertyId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
