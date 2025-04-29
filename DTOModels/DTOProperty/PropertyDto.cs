using RentMateAPI.Data.Models;

namespace RentMateAPI.DTOModels.DTOProperty
{
    public class PropertyDto
    {
        public int Id { get; set; }

        public int? LandlordId { get; set; }

        public string LandlordName { get; set; } = null!;
        public byte[]? LandlordImage { get; set; }

        public string Title { get; set; } = null!;

        public byte[] MainImage { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public string Location { get; set; } = null!;

        public int? Views { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? CreateAt { get; set; }

        public string PropertyApproval { get; set; } = null!;

        public virtual ICollection<PropertyImageDto> PropertyImages { get; set; } = new List<PropertyImageDto>();

    }
}
