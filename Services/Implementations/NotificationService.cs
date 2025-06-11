using Microsoft.AspNetCore.SignalR;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTONotification;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using RentMateAPI.Validations.Interfaces;

namespace RentMateAPI.Services.Implementations
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<NotificationHub> _notificationHubContext;
        private readonly IModelValidator<Notification> _notificationValidator;
        private readonly IModelValidator<User> _userValidator;

        public NotificationService(IUnitOfWork unitOfWork, IHubContext<NotificationHub> notificationHubContext, 
            IModelValidator<User> userValidator, IModelValidator<Notification> notificationValidator)
        {
            _unitOfWork = unitOfWork;
            _notificationHubContext = notificationHubContext;
            _userValidator = userValidator;
            _notificationValidator = notificationValidator;
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
            await _userValidator.IsModelExist(userId);

            var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == userId);
            
            if (notifications.Count == 0)
                throw new Exception("No notifications found for the user.");

            foreach (var notification in notifications)
                _unitOfWork.Notifications.Delete(notification.Id);
            
            await _unitOfWork.CompleteAsync();
            
        }

        public async Task<List<GetNotificationsDto>> GetNotificationsAsync(int userId)
        {
            await _userValidator.IsModelExist(userId);


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
            await _userValidator.IsModelExist(userId);


            var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == userId);
            return notifications.Any(n => n.Seen == 0);
        }

        public async Task MarkAsSeen(int notificationId)
        {
            var notification = await _notificationValidator.IsModelExistReturn(notificationId);

            notification.Seen = 1;
            await _unitOfWork.CompleteAsync();
        }

        public async Task<int> NumberOfUnSeenNotifications(int userId)
        {
            await _userValidator.IsModelExist(userId);


            var notifications = await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == userId);
            int count = notifications.Count(n => n.Seen == 0);
            if (NotificationHub.UserConnections.TryGetValue(userId, out string connectionId))
            {
                await _notificationHubContext.Clients.Client(connectionId).SendAsync("ReceiveUnseenCount", new {NumberOfUnSeenNotification = count });
            }
            return count;
        }
    }
}
