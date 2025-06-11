namespace RentMateAPI.Validations.Interfaces
{
    public interface IImageValidator
    {
        bool IsNullImage(IFormFile image);
        bool IsNullImage(IEnumerable<IFormFile> secondaryImages);

        bool IsValidImageExtension(IFormFile image);
        bool IsValidImageExtension(IFormFile mainImage, IEnumerable<IFormFile> secondaryImages);

        bool IsValidImageSize(IFormFile image);
        bool IsValidImageSize(IFormFile mainImageSize, IEnumerable<IFormFile> secondaryImages);
    }
}
