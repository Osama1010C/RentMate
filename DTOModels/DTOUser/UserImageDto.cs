namespace RentMateAPI.DTOModels.DTOUser
{
    public class UserImageDto
    {
        public int Id { get; set; }
        public IFormFile Image { get; set; } = null!;

    }
}
