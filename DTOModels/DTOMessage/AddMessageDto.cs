namespace RentMateAPI.DTOModels.DTOMessage
{
    public class AddMessageDto
    {
        public int senderId { get; set; }
        public int recieverId { get; set; }
        public string? message { get; set; }
    }
}
