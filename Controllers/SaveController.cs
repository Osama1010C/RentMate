using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    [Authorize(Roles = "tenant")]

    public class SaveController : ControllerBase
    { 
        private readonly ISavedPostService _savedPostService;
        public SaveController(ISavedPostService savedPostService)
        {
            this._savedPostService = savedPostService;
        }


        [HttpGet("Posts/{tenantId}")]
        public async Task<IActionResult> GetAllTenantSavedPosts(int tenantId)
        {
            try
            {
                var savedPosts = await _savedPostService.GetAllSavedAsync(tenantId);
                return Ok(savedPosts);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        [HttpPost]
        public async Task<IActionResult> SavePost(int tenantId, int propertyId)
        {
            try
            {
                await _savedPostService.SavePostAsync(tenantId, propertyId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
