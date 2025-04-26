using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace RentMateAPI.DTOModels.DTOUser
{
    public class UserDto
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string Password { get; set; } = null!;

        public byte[]? Image { get; set; }

        public string Role { get; set; } = null!;
    }
}
