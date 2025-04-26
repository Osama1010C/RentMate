using Microsoft.AspNetCore.Mvc;
using RentMateAPI.DTOModels.DTOImage;
using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    public class PropertyController : ControllerBase
    {
        private readonly IPropertyService _propertyService;
        public PropertyController(IPropertyService propertyService)
        {
            this._propertyService = propertyService;
        }


        [HttpGet]
        public async Task<IActionResult> GetAllProperties()
            => Ok(await _propertyService.GetAllAsync());
            


        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetPropertyDetails(int propertyId, int userId)
        {
            try
            {
                var property = await _propertyService.GetDetailsAsync(propertyId, userId);
                return Ok(property);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpGet("MyProperties/{tenantId}")]
        //[Authorize(Roles = "tenant")]
        public async Task<IActionResult> GetMyProperties(int tenantId)
        {
            try
            {
                var myProperties = await _propertyService.GetMyPropertiesAsync(tenantId);
                return Ok(myProperties);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost("AddProperty")]
        //[Authorize(Roles = "landlord")]
        public async Task<IActionResult> AddProperty([FromForm] AddPropertyDto propertyDto)
        {
            try
            {
                await _propertyService.AddAsync(propertyDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost("AddPropertyImage")]
        //[Authorize(Roles = "landlord")]
        public async Task<IActionResult> AddPropertyImage(int propertyId, [FromForm] AddPropertyImageDto propertyImageDto)
        {
            try
            {
                await _propertyService.AddImageAsync(propertyId, propertyImageDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        [HttpPut("ReplacePropertyMainImage/{propertyId}")]
        //[Authorize(Roles = "landlord")]
        public async Task<IActionResult> ReplacePropertyMainImage(int propertyId, [FromForm] ImageDto image)
        {
            try
            {
                await _propertyService.ReplaceMainImageAsync(propertyId, image);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }




        [HttpPut("Update/{propertyId}")]
        //[Authorize(Roles = "landlord")]
        public async Task<IActionResult> UpdateProperty(int propertyId, [FromBody] UpdatedPropertDto propertyDto)
        {
            try
            {
                await _propertyService.UpdateAsync(propertyId, propertyDto);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpDelete("Delete/{propertyId}")]
        //[Authorize(Roles = "landlord")]
        public async Task<IActionResult> DeleteProperty(int propertyId)
        {
            try
            {
                await _propertyService.DeleteAsync(propertyId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete("DeletePropertyImage/{propertyImageId}")]
        //[Authorize(Roles = "landlord")]
        public async Task<IActionResult> DeletePropertyImage(int propertyImageId)
        {
            try
            {
                await _propertyService.DeleteImageAsync(propertyImageId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}