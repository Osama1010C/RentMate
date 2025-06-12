using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    [Authorize (Roles ="admin")]
    public class HistoryController : ControllerBase
    {
        private readonly IHistoryService _historyService;
        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>Return all properties approval history to admin</summary>
        [HttpGet("PropertiesHistory")]
        public async Task<IActionResult> GetPropertiesHistory()
            => Ok(await _historyService.GetPropertiesHistoryAsync());


        /// <summary>Return all landlords registration approval history to admin</summary>
        [HttpGet("RegistrationsHistory")]
        public async Task<IActionResult> GetRegistrationLandlordsHistory()
            => Ok(await _historyService.GetLandlordsRegistrationHistoryAsync());


        /// <summary>Delete all properties approval history</summary>
        [HttpDelete("PropertiesHistory")]
        public async Task<IActionResult> DeletePropertiesHistory()
        {
            await _historyService.DeletePropertyHistory();
            return Ok();
        }

        /// <summary>Delete all landlords registration approval history</summary>
        [HttpDelete("RegistrationsHistory")]
        public async Task<IActionResult> DeleteRegistrationLandlordsHistory()
        {
            await _historyService.DeleteLandlordsRegistrationHistory();
            return Ok();
        }

    }
}
