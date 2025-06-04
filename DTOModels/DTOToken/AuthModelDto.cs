using System.Text.Json.Serialization;

namespace RentMateAPI.DTOModels.DTOToken
{
    public class AuthModelDto
    {
        
        public int Id { get; set; }
        public bool IsAuthenticated { get; set; }
        //public string Username { get; set; } = null!;
        //public string Email { get; set; } = null!;
        //public string Role { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime ExpiresOn { get; set; }
        public string RefreshToken { get; set; } = null!;
    }
}
