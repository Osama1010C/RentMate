namespace RentMateAPI.DTOModels.DTOToken
{
    public class RefreshTokenDto
    {
        public int UserId { get; set; }
        public string RefreshToken { get; set; } = "";
    }
}
