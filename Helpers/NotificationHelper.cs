using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Helpers
{
    public static class NotificationHelper
    {
        public static async Task<bool> IsMessagedBeforeInNotification(IUnitOfWork unitOfWork, string SenderName, int recieverId)
        {
            var notifications = await unitOfWork.Notifications.GetAllAsync(n => n.UserId == recieverId && n.NotificationType == "Message");
            return notifications.Any(n => n.Description.Contains(SenderName));
        }
    }
}
