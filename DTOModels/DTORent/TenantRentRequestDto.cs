namespace RentMateAPI.DTOModels.DTORent
{
    public class TenantRentRequestDto
    {
        public int RentId { get; set; }
        public string TenantName { get; set; } = null!;

        public string PropertyTitle { get; set; } = null!;
        public string RentStatus { get; set; } = null!;

        public byte[] PropertyMainImage { get; set; } = null!;

        public DateTime? CreateAt { get; set; }
    }
}
