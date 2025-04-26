namespace RentMateAPI.DTOModels.DTOMessage
{
    public class MessageDto
    {
        public string SenderName { get; set; } = null!;

        public string Content { get; set; } = null!;

        public DateTime? SentAt { get; set; }
    }
}
