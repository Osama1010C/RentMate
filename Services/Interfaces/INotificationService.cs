using RentMateAPI.DTOModels.DTONotification;

namespace RentMateAPI.Services.Interfaces
{
    public interface INotificationService
    {
        Task AddNotificationAsync(int userId, AddNotificationDto notificationDto);
        Task<List<GetNotificationsDto>> GetNotificationsAsync(int userId);
        Task DeleteNotificationAsync(int userId);
    }
}
