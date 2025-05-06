namespace RentMateAPI.DTOModels.DTORent
{
    public class RentPropertyDto
    {
        public int TenantId { get; set; }

        public int PropertyId { get; set; }

        public IFormFile? RequirmentDocument { get; set; }

    }
}
