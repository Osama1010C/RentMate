namespace RentMateAPI.Validations.Interfaces
{
    public interface IFileValidator
    {
        bool IsNullFile(IFormFile file);

        bool IsValidFileExtension(IFormFile file, string validExtension);

        bool IsValidFileSize(IFormFile file, long size);
    }
}
