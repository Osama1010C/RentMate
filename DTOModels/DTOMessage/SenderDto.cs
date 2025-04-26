namespace RentMateAPI.DTOModels.DTOMessage
{
    public class SenderDto
    {
        public int SenderId { get; set; }

        public string SenderName { get; set; } = null!;

        public byte[]? SenderImage { get; set; }
    }
}
