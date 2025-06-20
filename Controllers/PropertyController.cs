using Microsoft.AspNetCore.Authorization;
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


        /// <summary>Return list of all properties.</summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProperties() => Ok(await _propertyService.GetAllAsync());



        /// <summary>Take page number as input and return paginated list of properties.</summary>
        [HttpGet("Page/{pageNumber}")]
        public async Task<IActionResult> GetPagedProperties(int pageNumber) => Ok(await _propertyService.GetPageAsync(pageNumber));


        /// <summary>Return number of pages calculated by number of all properties</summary>
        [HttpGet("NumberOfPages")]
        public async Task<IActionResult> GetNumberOfPages() => Ok(await _propertyService.GetNumberOfPages());


        /// <summary>Take page number as input and return paginated list of properties for a specific LandLord</summary>
        [HttpGet("{landlordId}/Page/{pageNumber}")]
        [Authorize(Roles = "landlord")]
        public async Task<IActionResult> GetPagedPropertiesForLandlord(int landlordId, int pageNumber)
        {
            try
            {
                var properties = await _propertyService.GetLandlordPropertiesPageAsync(landlordId, pageNumber);
                return Ok(properties);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }



        /// <summary>Return number of pages calculated by number of all properties for a specific LandLord</summary>
        [HttpGet("{landlordId}/NumberOfPages")]
        [Authorize(Roles = "landlord")]
        public async Task<IActionResult> GetNumberOfPagesForLandlord(int landlordId)
        {
            try
            {
                return Ok(await _propertyService.GetNumberOfPagesForLandlord(landlordId));

            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

            /// <summary>Take propertyId and userId then return this property and add 1 view of this user to property</summary>
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


        /// <summary>Return all properties that is rented succefully for a specific tenant</summary>
        [HttpGet("MyProperties/{tenantId}")]
        [Authorize(Roles = "tenant")]
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




        /// <summary>Let landlord to add new property</summary>
        [HttpPost("AddProperty")]
        [Authorize(Roles = "landlord")]
        public async Task<IActionResult> AddProperty([FromForm] AddPropertyDto propertyDto, [FromForm] PropertyImagesDto images)
        {
            try
            {
                return Ok(await _propertyService.AddAsync(propertyDto, images));
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>add secondary image for property</summary>
        [HttpPost("AddPropertyImage")]
        [Authorize(Roles = "landlord")]
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


        /// <summary>Update property</summary>
        [HttpPut("Update/{propertyId}")]
        [Authorize(Roles = "landlord")]
        public async Task<IActionResult> UpdateProperty(int propertyId, [FromForm]UpdatedPropertDto propertyDto, [FromForm]ImageDto? image = null)
        {
            try
            {
                await _propertyService.UpdatePropertyAsync(propertyId, propertyDto, image);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>Delete property</summary>
        [HttpDelete("Delete/{propertyId}")]
        [Authorize(Roles = "landlord")]
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


        /// <summary>Delete secondary property image</summary>
        [HttpDelete("DeletePropertyImage/{propertyImageId}")]
        [Authorize(Roles = "landlord")]
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