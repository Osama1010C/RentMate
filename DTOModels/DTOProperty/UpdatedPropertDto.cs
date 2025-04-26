namespace RentMateAPI.DTOModels.DTOProperty
{
    public class UpdatedPropertDto
    {
        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public string Location { get; set; } = null!;
    }
}
