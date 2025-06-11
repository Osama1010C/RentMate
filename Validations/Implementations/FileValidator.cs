using RentMateAPI.Exceptions;
using RentMateAPI.Validations.Interfaces;

namespace RentMateAPI.Validations.Implementations
{
    public class FileValidator : IFileValidator
    {
        public bool IsNullFile(IFormFile file)
        {
            if(file == null || file.Length == 0)
                throw new ArgumentNullException("File cannot be null or empty.");
            return true;
        }

        public bool IsValidFileExtension(IFormFile file, string validExtension)
        {
            var extension = Path.GetExtension(file.FileName).ToLower();
            
            if(extension != validExtension.ToLower())
                throw new InvalidExtensionException($"Invalid file extension: {extension}. Expected: {validExtension}.");

            return true;
        }

        public bool IsValidFileSize(IFormFile file, long size)
        {
            if (file.Length > size) throw new ExceedLimitSizeException("File size exceeds the maximum limit of 200 Byte.");
            
            return true;
        }
    }
}
