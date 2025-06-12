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

        /// <summary>Return all notifications for a specific user</summary>
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

        /// <summary>Return number of unseen notifications for a specific user</summary>
        [HttpGet("NumberOfUnSeen")]
        public async Task<IActionResult> GetNumberOfUnSeenNotifications(int userId)
        {
            try
            {
                var unseenCount = await _notificationService.NumberOfUnSeenNotifications(userId);
                return Ok(new { NumOfUnSeenNotififcations = unseenCount });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        /// <summary>Check if there is unseen notifications</summary>
        [HttpGet("CheckUnSeen")]
        public async Task<IActionResult> CheckUnseenNotifications(int userId)
        {
            try
            {
                var unseenCount = await _notificationService.IsAnyUnSeenNotification(userId);
                return Ok(new { IsAnyUnSeen = unseenCount });
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>Mark specific notification as seen</summary>
        [HttpPost("MarkAsSeen")]
        public async Task<IActionResult> MarkAsSeen(int notificationId)
        {
            try
            {
                await _notificationService.MarkAsSeen(notificationId);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>Delete all notifications for a specific user</summary>
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
