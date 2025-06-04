using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    [Authorize(Roles = "tenant,landlord")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;
        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> GetNotifications(int userId)
        {
            try
            {
                var notifications = await _notificationService.GetNotificationsAsync(userId);
                return Ok(notifications);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteNotifications(int userId)
        {
            try
            {
                await _notificationService.DeleteNotificationAsync(userId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


    }
}
