namespace RentMateAPI.DTOModels.DTOProperty
{
    public class AddPropertyDto
    {
        public int? LandlordId { get; set; }

        public string Title { get; set; } = null!;

        public IFormFile MainImage { get; set; } = null!;

        public string Description { get; set; } = null!;

        public decimal Price { get; set; }

        public string Location { get; set; } = null!;

    }
}
