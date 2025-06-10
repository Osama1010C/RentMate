using Microsoft.AspNetCore.SignalR;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTONotification;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _notificationHubContext;

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> notificationHubContext)
        {
            _unitOfWork = unitOfWork;
            _notificationHubContext = notificationHubContext;
        }


        public async Task AddNotificationAsync(int userId, AddNotificationDto notificationDto)
        {
            await _unitOfWork.Notifications.AddAsync(new()
            {
                UserId = userId,
                Description = notificationDto.Description,
                NotificationType = notificationDto.NotificationType,
                NotificationTypeId = notificationDto.NotificationTypeId,
            });
            if (NotificationHub.UserConnections.TryGetValue(userId, out string connectionId))
            {
                await _notificationHubContext.Clients.Client(connectionId).SendAsync("ReceiveNotification", notificationDto);
            }
        }

        public async Task DeleteNotificationAsync(int userId)
        {
            if (!await IsUserExistAsync(userId))
                throw new Exception("User does not exist.");

            var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == userId);
            
            if (notifications.Count == 0)
                throw new Exception("No notifications found for the user.");

            foreach (var notification in notifications)
                _unitOfWork.Notifications.Delete(notification.Id);
            
            await _unitOfWork.CompleteAsync();
            
        }

        public async Task<List<GetNotificationsDto>> GetNotificationsAsync(int userId)
        {
            if (!await IsUserExistAsync(userId))
                throw new Exception("User does not exist.");

            var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == userId);

            return notifications.Select(n => new GetNotificationsDto
            {
                Id = n.Id,
                Description = n.Description,
                NotificationType = n.NotificationType,
                NotificationTypeId = n.NotificationTypeId,
                IsSeen = n.Seen == 1,
                ActionDate = n.ActionDate
            })
                .OrderByDescending(gn => gn.ActionDate)
                .ToList();
        }

        

        public async Task<bool> IsAnyUnSeenNotification(int userId)
        {
            if (!await IsUserExistAsync(userId))
                throw new Exception("User does not exist.");

            var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == userId);
            return notifications.Any(n => n.Seen == 0);
        }

        public async Task MarkAsSeen(int notificationId)
        {
            var notification = await _unitOfWork.Notifications.GetByIdAsync(notificationId);
            if (notification == null)
                throw new Exception("Notification does not exist.");

            notification.Seen = 1;
            await _unitOfWork.CompleteAsync();
        }

        public async Task<int> NumberOfUnSeenNotifications(int userId)
        {
            if (!await IsUserExistAsync(userId))
                throw new Exception("User does not exist.");

            var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == userId);
            int count = notifications.Count(n => n.Seen == 0);
            if (NotificationHub.UserConnections.TryGetValue(userId, out string connectionId))
            {
                await _notificationHubContext.Clients.Client(connectionId).SendAsync("ReceiveUnseenCount", new {NumberOfUnSeenNotification = count });
            }
            return count;
        }

        private async Task<bool> IsUserExistAsync(int userId) => await _unitOfWork.Users.IsExistAsync(userId);

        //private void MakeAsSeen(List<Notification> notifications)
        //{
        //    foreach (var notification in notifications)
        //        notification.Seen = 1;
        //}

    }
}
