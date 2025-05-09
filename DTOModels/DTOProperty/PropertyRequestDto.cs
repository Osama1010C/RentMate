namespace RentMateAPI.DTOModels.DTOProperty
{
    public class PropertyRequestDto
    {
        public int PropertyId { get; set; }
        public int RentId { get; set; }
        public string TenantName { get; set; } = null!;
        public string PropertyTitle { get; set; } = null!;
        public string RentStatus { get; set; } = null!;
        public byte[] PropertyMainImage { get; set; } = null!;
        public DateTime? RequestCreateAt { get; set; }
        public int? LandlordId { get; set; }
        public string LandlordName { get; set; } = null!;
        public byte[] LandlordImage { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Location { get; set; } = null!;
        public decimal Price { get; set; }
        public string Status { get; set; } = null!;
        public int? Views { get; set; }
        public DateTime? PropertyCreateAt { get; set; }
        public List<PropertyImageDto> PropertyImages { get; set; } = null!;
    }
}
