namespace RentMateAPI.DTOModels.DTOUser
{
    public class PendingLandlordsDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public byte[]? Image { get; set; }

    }
}
