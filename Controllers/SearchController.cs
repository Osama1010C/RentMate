using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    

    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        public SearchController(ISearchService searchService)
        {
            this._searchService = searchService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllProperties(string? location = null, decimal? fromPrice = null, decimal? toPrice = null)
        {
            var properties = await _searchService.SearchAsync(location,fromPrice, toPrice);

            return Ok(properties);
        }
    }
}
