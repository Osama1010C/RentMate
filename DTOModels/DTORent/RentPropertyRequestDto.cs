namespace RentMateAPI.DTOModels.DTORent
{
    public class RentPropertyRequestDto
    {
        public int RentId { get; set; }
        public string TenantName { get; set; } = null!;

        public string PropertyTitle { get; set; } = null!;

        public byte[] PropertyMainImage { get; set; } = null!;

        public List<string> RequirmentDocument { get; set; } = new List<string>();

        public DateTime? CreateAt { get; set; }
    }
}
