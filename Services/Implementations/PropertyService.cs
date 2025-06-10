using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOImage;
using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using Property = RentMateAPI.Data.Models.Property;

namespace RentMateAPI.Services.Implementations
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PropertyService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        

        public async Task<List<PropertyDto>> GetAllAsync()
        {
            var properties = await _unitOfWork.Properties.GetAllAsync(includeProperties:"Landlord");

            var propertyDtos = properties.Select(property => new PropertyDto
            {
                Id = property.Id,
                LandlordId = property.LandlordId,
                LandlordName = property.Landlord!.Name,
                LandlordImage = property.Landlord.Image,
                Title = property.Title,
                Description = property.Description,
                Location = property.Location,
                Price = property.Price,
                Status = property.Status,
                Views = property.Views,
                MainImage = property.MainImage,
                CreateAt = property.CreateAt,
                PropertyImages = GetPropertyImagesAsync(property.Id).Result,
                PropertyApproval = property.PropertyApproval
            }).OrderByDescending(p => p.Views).ToList();

            return propertyDtos;
        }

        public async Task<PropertyDetailsDto> GetDetailsAsync(int propertyId, int userId)
        {
            if(await _unitOfWork.Users.GetByIdAsync(userId) == null) 
                throw new Exception($"User with Id {userId} not found!");

            var property = await _unitOfWork.Properties.GetAsync(p => p.Id == propertyId , includeProperties:"Landlord");
            if (property is null) 
                throw new Exception($"Property with Id {propertyId} not exist!");

            var propertyDto = new PropertyDetailsDto
            {
                Id = property.Id,
                LandlordId = property.LandlordId,
                LandlordName = property.Landlord!.Name,
                LandlordImage = property.Landlord.Image,
                Title = property.Title,
                Description = property.Description,
                Location = property.Location,
                Price = property.Price,
                Status = property.Status,
                Views = property.Views,
                MainImage = property.MainImage,
                CreateAt = property.CreateAt,
                PropertyImages = await GetPropertyImagesAsync(propertyId),
                PropertyApproval = property.PropertyApproval,
                IsSaved = !await IsNewSavedPostAsync(userId, propertyId),
                IsAskForRent = await IsAskedForRentAsync(userId, propertyId)
            };

            if (await IsNewViewAsync(userId, propertyId))
            {
                await _unitOfWork.PropertyViews.AddAsync(new()
                {
                    UserId = userId,
                    PropertyId = propertyId
                });

                property!.Views++;

                await _unitOfWork.CompleteAsync();
            }

            return propertyDto;
        }

        public async Task<List<PropertyDto>> GetMyPropertiesAsync(int tenantId)
        {
            if (await _unitOfWork.Users.GetByIdAsync(tenantId) == null)
                throw new Exception($"User with Id {tenantId} not found!");


            var properties = await _unitOfWork.TenantProperties.GetAllAsync(p => p.TenantId == tenantId, includeProperties:"Property");


            var propertyDtos = properties.Select(p =>
                {
                    var landlordName = _unitOfWork.Users.GetByIdAsync(p.Property.LandlordId).Result!.Name;
                    var landlordImage = _unitOfWork.Users.GetByIdAsync(p.Property.LandlordId).Result!.Image;
                    return new PropertyDto
                    {
                        Id = p.Property.Id,
                        LandlordId = p.Property.LandlordId,
                        LandlordName = landlordName,
                        LandlordImage = landlordImage,
                        Title = p.Property.Title,
                        Description = p.Property.Description,
                        Location = p.Property.Location,
                        Price = p.Property.Price,
                        Status = p.Property.Status,
                        Views = p.Property.Views,
                        MainImage = p.Property.MainImage,
                        CreateAt = p.Property.CreateAt,
                        PropertyImages = GetPropertyImagesAsync(p.Property.Id).Result,
                        PropertyApproval = p.Property.PropertyApproval
                    };
                }).ToList();
            

            return propertyDtos;
        }

        

        public async Task<int> AddAsync(AddPropertyDto propertyDto, PropertyImagesDto imagesDto)
        {
            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
            if (propertyDto.MainImage == null)
                throw new Exception($"Property must have main image!");

            if (!imagesDto.Images.Any())
                throw new Exception($"Property must have at least one secondary image!");

            if (!IsValidFileExtension(GetFileExtension(propertyDto.MainImage), GetFilesExtension(imagesDto.Images), allowedExtensions))
                throw new Exception($"Invalid file extension! Allowed extensions are: {string.Join(", ", allowedExtensions)}");

            if(!IsValidFileSize(GetFileSize(propertyDto.MainImage), GetFilesSize(imagesDto.Images), 1*1024*1024))
                throw new Exception("File size exceeds the maximum limit of 1MB.");



            var landlord = await _unitOfWork.Users.GetByIdAsync(propertyDto.LandlordId);
            if (landlord is null)
                throw new Exception($"Landlord with Id {propertyDto.LandlordId} not found!");

            if (landlord.Role != "landlord")
                throw new Exception($"This User Does Not allowed to Add Properties");


            byte[]? mainImageBytes = null;
            using (var memoryStream = new MemoryStream())
            {
                await propertyDto.MainImage.CopyToAsync(memoryStream);
                mainImageBytes = memoryStream.ToArray();
            }

            
            var property = new Property
            {
                LandlordId = propertyDto.LandlordId,
                Title = propertyDto.Title,
                Description = propertyDto.Description,
                Location = propertyDto.Location,
                Price = propertyDto.Price,
                MainImage = mainImageBytes,
                Views = 0,
                Status = "available",
                PropertyApproval = "pending"
            };

            
            await _unitOfWork.Properties.AddAsync(property);
            await _unitOfWork.CompleteAsync(); 

            
            if (imagesDto?.Images != null && imagesDto.Images.Any())
            {
                foreach (var formFile in imagesDto.Images)
                {
                    using var memoryStream = new MemoryStream();
                    await formFile.CopyToAsync(memoryStream);

                    var propertyImage = new PropertyImage
                    {
                        PropertyId = property.Id, 
                        Image = memoryStream.ToArray()
                    };

                    await _unitOfWork.PropertyImages.AddAsync(propertyImage);
                }

                await _unitOfWork.CompleteAsync(); 
            }

            return property.Id;
        }

        private bool IsValidFileExtension(string ImageExtension, List<string> allowedExtensions)
        {
            if (!allowedExtensions.Contains(ImageExtension.ToLower()))
                return false;

            return true;
        }
        private bool IsValidFileExtension(string mainImageExtension, List<string> secondaryImagesExtensions, List<string> allowedExtensions)
        {
            if(!allowedExtensions.Contains(mainImageExtension.ToLower()))
                return false;

            foreach(var image in secondaryImagesExtensions)
                if(!allowedExtensions.Contains(image.ToLower()))
                    return false;

            return true;
        }

        private bool IsValidFileSize(long ImageSize, long allowedSize)
        {
            if (ImageSize > allowedSize) return false;
            return true;
        }
        private bool IsValidFileSize(long mainImageSize, List<long> secondaryImagesSize, long allowedSize)
        {
            if (mainImageSize > allowedSize) return false;
            foreach (var size in secondaryImagesSize)
                if(size > allowedSize) return false;

            return true;
        }

        private List<long> GetFilesSize(IEnumerable<IFormFile> files)
        {
            var sizes = new List<long>();
            foreach (var file in files)
                sizes.Add(file.Length);
            return sizes;
        }
        private long GetFileSize(IFormFile file)
            => file.Length;
        private List<string> GetFilesExtension(IEnumerable<IFormFile> files)
        {
            var extensions = new List<string>();
            foreach (var file in files)
                extensions.Add(Path.GetExtension(file.FileName).ToLower());
            return extensions;
        }
        private string GetFileExtension(IFormFile file)
            => Path.GetExtension(file.FileName).ToLower();





        public async Task UpdatePropertyAsync(int propertyId, UpdatedPropertDto propertyDto, ImageDto? image = null)
        {
            var property = await _unitOfWork.Properties.GetAsync(p => p.Id == propertyId && p.Status != "rented");
            if (property is null)
                throw new Exception($"Property with id: {propertyId} not found or not available");

            
            if (!string.IsNullOrWhiteSpace(propertyDto.Title))
                property.Title = propertyDto.Title;

            if (!string.IsNullOrWhiteSpace(propertyDto.Description))
                property.Description = propertyDto.Description;

            if (!string.IsNullOrWhiteSpace(propertyDto.Location))
                property.Location = propertyDto.Location;

            if (propertyDto.Price.HasValue)
                property.Price = propertyDto.Price.Value;

           
            if (image != null && image.Image != null)
            {
                var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
                if (!IsValidFileExtension((GetFileExtension(image.Image)), allowedExtensions))
                    throw new Exception($"Invalid file extension! Allowed extensions are: {string.Join(", ", allowedExtensions)}");
                if (!IsValidFileSize(GetFileSize(image.Image), 1 * 1024 * 1024))
                    throw new Exception("File size exceeds the maximum limit of 1MB.");
                using var memoryStream = new MemoryStream();
                image.Image.CopyTo(memoryStream);
                property.MainImage = memoryStream.ToArray();
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task AddImageAsync(int propertyId, AddPropertyImageDto propertyImageDto)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property is null) throw new Exception($"Property with id : {propertyId} not found or not availble");
            byte[]? propertyImage = null;

            if(propertyImageDto.Image == null)
                throw new Exception($"Please send image");

            var allowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
            if (!IsValidFileExtension((GetFileExtension(propertyImageDto.Image)), allowedExtensions))
                throw new Exception($"Invalid file extension! Allowed extensions are: {string.Join(", ", allowedExtensions)}");
            if (!IsValidFileSize(GetFileSize(propertyImageDto.Image), 1 * 1024 * 1024))
                throw new Exception("File size exceeds the maximum limit of 1MB.");

            // read image
            using var memoryStream = new MemoryStream();
            propertyImageDto.Image.CopyTo(memoryStream);
            propertyImage = memoryStream.ToArray();
            var propertyImageEntity = new PropertyImage
            {
                PropertyId = propertyId,
                Image = propertyImage
            };
            await _unitOfWork.PropertyImages.AddAsync(propertyImageEntity);
            await _unitOfWork.CompleteAsync();
        }


        public async Task DeleteAsync(int propertyId)
        {
            var prop = await _unitOfWork.Properties.GetAsync(p => p.Id == propertyId && p.Status != "rented");
            if (prop is null) throw new Exception($"Property with id : {propertyId} not found or not availble");

            _unitOfWork.Properties.Delete(prop.Id);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteImageAsync(int propertyImageId)
        {
            // propertyImageId is the primary key of PropertyImage Table
            var propertyImage = await _unitOfWork.PropertyImages.GetByIdAsync(propertyImageId);
            if (propertyImage is null) throw new Exception($"PropertyImage with id : {propertyImageId} not found or not availble");

            _unitOfWork.PropertyImages.Delete(propertyImageId);
            await _unitOfWork.CompleteAsync();
        }






        private async Task<bool> IsNewViewAsync(int userId, int propertyId)
        {
            var propertyView = await _unitOfWork.PropertyViews.GetAsync(p => (p.UserId == userId) && (p.PropertyId == propertyId));

            return propertyView is null;
        }



        private async Task<List<PropertyImageDto>> GetPropertyImagesAsync(int propertyId)
        {
            var images = await _unitOfWork.PropertyImages.GetAllAsync(p => p.PropertyId == propertyId);
            var result = images.Select(m => new PropertyImageDto
            {
                PropertyImageId = m.Id,
                Image = m.Image
            });
            return result.ToList();
        }

        private async Task<bool> IsNewSavedPostAsync(int tenantId, int propertyId)
        {
            var savedPost = await _unitOfWork.SavedPosts
                        .GetAllAsync(p => (p.TenantId == tenantId) && (p.PropertyId == propertyId));

            return savedPost.Count() == 0 ? true : false;
        }

        private async Task<bool> IsAskedForRentAsync(int tenantId, int propertyId)
        {
            var rentingProperty = await _unitOfWork.RentalRequests
                        .GetAllAsync(r => (r.TenantId == tenantId) && (r.PropertyId == propertyId) && (r.Status == "pending"));

            return rentingProperty.Count() > 0;
        }

    }
}
