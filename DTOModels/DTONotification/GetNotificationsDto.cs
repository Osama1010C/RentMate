namespace RentMateAPI.DTOModels.DTONotification
{
    public class GetNotificationsDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = null!;

        public string NotificationType { get; set; } = null!;

        public int NotificationTypeId { get; set; }

        public DateTime? ActionDate { get; set; }
    }
}
