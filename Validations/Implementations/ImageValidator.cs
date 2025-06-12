using RentMateAPI.DTOModels.DTOImage;
using RentMateAPI.Exceptions;
using RentMateAPI.Validations.Interfaces;

namespace RentMateAPI.Validations.Implementations
{
    public class ImageValidator : IImageValidator
    {
        private static readonly List<string> AllowedExtensions = new List<string> { ".jpg", ".jpeg", ".png", ".avif" };
        private static readonly long AllowedSize = 1 * 1024 * 1024; //1MB

        public bool IsNullImage(IFormFile image) 
        {
            if(image == null || image.Length == 0)
                throw new ArgumentNullException("Please send image");
            return true;
        }

        public bool IsNullImage(IEnumerable<IFormFile> secondaryImages)
        {
            if (!secondaryImages.Any())
                throw new ArgumentNullException($"Property must have at least one secondary image!");
            return true;
        }

        public bool IsValidImageExtension(IFormFile image)
        {
            var extension = Path.GetExtension(image.FileName);
            if(!AllowedExtensions.Contains(extension.ToLower()))
                throw new InvalidExtensionException($"Invalid file extension: {extension}. Allowed extensions are: {string.Join(", ", AllowedExtensions)}");
            return true;
        }

        public bool IsValidImageExtension(IFormFile mainImage, IEnumerable<IFormFile> secondaryImages)
        {
            if (!AllowedExtensions.Contains(GetFileExtension(mainImage)))
                throw new InvalidExtensionException($"Invalid file extension! Allowed extensions are: {string.Join(", ", AllowedExtensions)}");


            foreach (var image in GetFilesExtension(secondaryImages))
                if (!AllowedExtensions.Contains(image.ToLower()))
                    throw new InvalidExtensionException($"Invalid file extension! Allowed extensions are: {string.Join(", ", AllowedExtensions)}");

            return true;
        }

        public bool IsValidImageSize(IFormFile image)
        {
            if(image.Length > AllowedSize)
                throw new ExceedLimitSizeException("File size exceeds the maximum limit of 1MB.");
            return true;
        }

        public bool IsValidImageSize(IFormFile mainImageSize, IEnumerable<IFormFile> secondaryImages)
        {
            if (GetFileSize(mainImageSize) > AllowedSize) throw new ExceedLimitSizeException("File size exceeds the maximum limit of 1MB.");
            foreach (var size in GetFilesSize(secondaryImages))
                if (size > AllowedSize) throw new ExceedLimitSizeException("File size exceeds the maximum limit of 1MB.");

            return true;
        }



        // Helper methods to get file extension and size
        private string GetFileExtension(IFormFile file) => Path.GetExtension(file.FileName).ToLower();

        private List<string> GetFilesExtension(IEnumerable<IFormFile> files)
        {
            var extensions = new List<string>();
            foreach (var file in files)
                extensions.Add(Path.GetExtension(file.FileName).ToLower());
            return extensions;
        }
        private long GetFileSize(IFormFile file) => file.Length;

        private List<long> GetFilesSize(IEnumerable<IFormFile> files)
        {
            var sizes = new List<long>();
            foreach (var file in files)
                sizes.Add(file.Length);
            return sizes;
        }
    }
}
