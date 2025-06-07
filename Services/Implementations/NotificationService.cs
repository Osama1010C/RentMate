using Microsoft.AspNetCore.SignalR;
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
            //await _unitOfWork.CompleteAsync();
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
                ActionDate = n.ActionDate
            })
                .OrderByDescending(gn => gn.ActionDate)
                .ToList();
        }


        private async Task<bool> IsUserExistAsync(int userId) => await _unitOfWork.Users.IsExistAsync(userId);

    }
}
