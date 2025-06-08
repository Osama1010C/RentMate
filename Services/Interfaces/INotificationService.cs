using RentMateAPI.DTOModels.DTONotification;

namespace RentMateAPI.Services.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(int userId, AddNotificationDto notificationDto);
        Task<List<GetNotificationsDto>> GetNotificationsAsync(int userId);
        Task DeleteNotificationAsync(int userId);

        Task<int> NumberOfUnSeenNotifications(int userId);
        Task<bool> IsAnyUnSeenNotification(int userId);

        Task MarkAsSeen(int notificationId);
    }
}
