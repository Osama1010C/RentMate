using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOImage;
using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.Helpers;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using RentMateAPI.Validations.Interfaces;

namespace RentMateAPI.Services.Implementations
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IImageValidator _imageValidator;
        private readonly IModelValidator<User> _userValidator;
        private readonly IModelValidator<Property> _propertyValidator;
        private readonly IModelValidator<PropertyImage> _propertyImagesValidator;

        public PropertyService(IUnitOfWork unitOfWork, IImageValidator imageValidator,
            IModelValidator<User> userValidator, IModelValidator<Property> propertyValidator,
            IModelValidator<PropertyImage> propertyImagesValidator)
        {
            this._unitOfWork = unitOfWork;
            this._imageValidator = imageValidator;
            this._userValidator = userValidator;
            this._propertyValidator = propertyValidator;
            this._propertyImagesValidator = propertyImagesValidator;
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
                //PropertyImages = GetPropertyImagesAsync(property.Id).Result,
                PropertyImages = PropertyImageHelper.GetPropertyImagesAsync(_unitOfWork, property.Id).Result,
                PropertyApproval = property.PropertyApproval
            }).OrderByDescending(p => p.Views).ToList();

            return propertyDtos;
        }

        public async Task<PropertyDetailsDto> GetDetailsAsync(int propertyId, int userId)
        {
            await _userValidator.IsModelExist(userId);

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
                PropertyImages = PropertyImageHelper.GetPropertyImagesAsync(_unitOfWork, property.Id).Result,
                PropertyApproval = property.PropertyApproval,
                IsSaved = !SavePostHelper.IsNewSavedPostAsync(_unitOfWork, userId, propertyId).Result,
                IsAskForRent = RentalHelper.IsAskedForRentAsync(_unitOfWork, userId, propertyId).Result
            };

            if (await PostViewHelper.IsNewViewAsync(_unitOfWork, userId, propertyId))
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
            var tenant = await _userValidator.IsModelExistReturn(tenantId);
            if (tenant.Role != "tenant") throw new Exception("There is no tenant with that id!");


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
                        PropertyImages = PropertyImageHelper.GetPropertyImagesAsync(_unitOfWork, p.Property.Id).Result,
                        PropertyApproval = p.Property.PropertyApproval
                    };
                }).ToList();
            

            return propertyDtos;
        }

        

        public async Task<int> AddAsync(AddPropertyDto propertyDto, PropertyImagesDto imagesDto)
        {
            _imageValidator.IsNullImage(propertyDto.MainImage);
            _imageValidator.IsNullImage(imagesDto.Images);
            _imageValidator.IsValidImageExtension(propertyDto.MainImage, imagesDto.Images);
            _imageValidator.IsValidImageSize(propertyDto.MainImage, imagesDto.Images);

            var landlord = await _userValidator.IsModelExistReturn((int)propertyDto.LandlordId!);
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
                _imageValidator.IsValidImageExtension(image.Image);
                _imageValidator.IsValidImageSize(image.Image);

                using var memoryStream = new MemoryStream();
                image.Image.CopyTo(memoryStream);
                property.MainImage = memoryStream.ToArray();
            }

            await _unitOfWork.CompleteAsync();
        }

        public async Task AddImageAsync(int propertyId, AddPropertyImageDto propertyImageDto)
        {
            var property = await _propertyValidator.IsModelExistReturn(propertyId);
            byte[]? propertyImage = null;

            _imageValidator.IsNullImage(propertyImageDto.Image!);
            _imageValidator.IsValidImageExtension(propertyImageDto.Image!);
            _imageValidator.IsValidImageSize(propertyImageDto.Image!);

            // read image
            using var memoryStream = new MemoryStream();
            propertyImageDto.Image!.CopyTo(memoryStream);
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
            var propertyImage = _propertyImagesValidator.IsModelExistReturn(propertyImageId);
            _unitOfWork.PropertyImages.Delete(propertyImageId);
            await _unitOfWork.CompleteAsync();
        }
    }
}
