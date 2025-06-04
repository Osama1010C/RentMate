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

        [HttpGet("PropertiesHistory")]
        public async Task<IActionResult> GetPropertiesHistory()
            => Ok(await _historyService.GetPropertiesHistoryAsync());


        [HttpGet("RegistrationsHistory")]
        public async Task<IActionResult> GetRegistrationLandlordsHistory()
            => Ok(await _historyService.GetLandlordsRegistrationHistoryAsync());


        [HttpDelete("PropertiesHistory")]
        public async Task<IActionResult> DeletePropertiesHistory()
        {
            await _historyService.DeletePropertyHistory();
            return Ok();
        }

        [HttpDelete("RegistrationsHistory")]
        public async Task<IActionResult> DeleteRegistrationLandlordsHistory()
        {
            await _historyService.DeleteLandlordsRegistrationHistory();
            return Ok();
        }

    }
}
