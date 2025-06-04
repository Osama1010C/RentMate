namespace RentMateAPI.DTOModels.DTONotification
{
    public class AddNotificationDto
    {
        public string Description { get; set; } = null!;

        public string NotificationType { get; set; } = null!;

        public int NotificationTypeId { get; set; }

    }
}
