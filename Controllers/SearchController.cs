using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    [Authorize(Roles = "admin,tenant,landlord")]

    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            this._searchService = searchService;
        }

        [HttpGet]
        //[Authorize(Roles = "admin,tenant,landlord")]
        public async Task<IActionResult> GetAllProperties(string? location = null, decimal? fromPrice = null, decimal? toPrice = null)
        {
            var properties = await _searchService.SearchAsync(location,fromPrice, toPrice);

            return Ok(properties);
        }

        //[HttpGet("Price")]
        //public async Task<IActionResult> GetAllPropertiesByPrice(decimal fromPrice, decimal toPrice)
        //{
        //    var properties = await _searchService.SearchByPriceAsync(fromPrice, toPrice);

        //    return Ok(properties);
        //}

        

        //[HttpGet("Location/{location}")]
        //public async Task<IActionResult> GetAllPropertiesByLocation(string location)
        //{
        //    var properties = await _searchService.SearchByLocationAsync(location);

        //    return Ok(properties);
        //}
    }
}
