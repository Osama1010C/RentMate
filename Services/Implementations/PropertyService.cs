using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOImage;
using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
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

            //var propertyDtos = properties.Select(property => new AllPropertyDto
            //{
            //    Id = property.Id,
            //    LandlordId = property.LandlordId,
            //    //LandlordName = property.Landlord!.Name,
            //    Title = property.Title,
            //    //Description = property.Description,
            //    Location = property.Location,
            //    Price = property.Price,
            //    Status = property.Status,
            //    Views = property.Views,
            //    MainImage = property.MainImage,
            //    //CreateAt = property.CreateAt,
            //    PropertyApproval = property.PropertyApproval
            //}).ToList();

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
            }).ToList();

            return propertyDtos;
        }

        public async Task<PropertyDto> GetDetailsAsync(int propertyId, int userId)
        {
            if(await _unitOfWork.Users.GetByIdAsync(userId) == null) 
                throw new Exception($"User with Id {userId} not found!");

            var property = await _unitOfWork.Properties.GetAsync(p => p.Id == propertyId , includeProperties:"Landlord");
            if (property is null) 
                throw new Exception($"Property with Id {propertyId} not exist!");

            var propertyDto = new PropertyDto
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
                PropertyApproval = property.PropertyApproval
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

        //public async Task<int> AddAsync(AddPropertyDto propertyDto)
        //{
        //    if(await _unitOfWork.Users.GetByIdAsync(propertyDto.LandlordId) is null)
        //        throw new Exception($"Landlord with Id {propertyDto.LandlordId} not found!");

        //    byte[]? propertyImage = null;

        //    // read main image
        //    using (var memoryStream = new MemoryStream())
        //    {
        //        propertyDto.MainImage.CopyTo(memoryStream);
        //        propertyImage = memoryStream.ToArray();

        //    }

        //    var property = new Property
        //    {
        //        LandlordId = propertyDto.LandlordId,
        //        Title = propertyDto.Title,
        //        Description = propertyDto.Description,
        //        Location = propertyDto.Location,
        //        Price = propertyDto.Price,
        //        MainImage = propertyImage,
        //        Views = 0,
        //        Status = "available",
        //        PropertyApproval = "pending"
        //    };
        //    await _unitOfWork.Properties.AddAsync(property);

        //    await _unitOfWork.CompleteAsync();
        //    return property.Id;
        //}

        public async Task<int> AddAsync(AddPropertyDto propertyDto, PropertyImagesDto imagesDto)
        {
            // 1. Validate landlord exists
            var landlord = await _unitOfWork.Users.GetByIdAsync(propertyDto.LandlordId);
            if (landlord is null)
                throw new Exception($"Landlord with Id {propertyDto.LandlordId} not found!");

            if (landlord.Role != "landlord")
                throw new Exception($"This User Does Not allowed to Add Properties");

            if(!imagesDto.Images.Any())
                throw new Exception($"Property must have at least one secondary image!");

            // 2. Read main image into bytes
            byte[]? mainImageBytes = null;
            using (var memoryStream = new MemoryStream())
            {
                await propertyDto.MainImage.CopyToAsync(memoryStream);
                mainImageBytes = memoryStream.ToArray();
            }

            // 3. Create Property entity
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

            // 4. Add property (first)
            await _unitOfWork.Properties.AddAsync(property);
            await _unitOfWork.CompleteAsync(); // Save to generate PropertyId

            // 5. Handle additional property images
            if (imagesDto?.Images != null && imagesDto.Images.Any())
            {
                foreach (var formFile in imagesDto.Images)
                {
                    using var memoryStream = new MemoryStream();
                    await formFile.CopyToAsync(memoryStream);

                    var propertyImage = new PropertyImage
                    {
                        PropertyId = property.Id, // newly created property ID
                        Image = memoryStream.ToArray()
                    };

                    await _unitOfWork.PropertyImages.AddAsync(propertyImage);
                }

                await _unitOfWork.CompleteAsync(); // Save all images
            }

            return property.Id;
        }



        //public async Task AddImageAsync(int propertyId, AddPropertyImageDto propertyImageDto)
        //{
        

        //    if (await _unitOfWork.Properties.GetByIdAsync(propertyId) is null)
        //        throw new Exception($"Property with Id {propertyId} not found!");


        //    byte[]? propertyImage = null;

        //    // read image
        //    using var memoryStream = new MemoryStream();


        //    propertyImageDto.Image!.CopyTo(memoryStream);
        //    propertyImage = memoryStream.ToArray();

        //    await _unitOfWork.PropertyImages.AddAsync(new()
        //    {
        //        PropertyId = propertyId,
        //        Image = propertyImage,
        //    });

        //    await _unitOfWork.CompleteAsync();
        //}

        public async Task ReplaceMainImageAsync(int propertyId, ImageDto image)
        {
            var property = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            if (property is null)
                throw new Exception($"Property with Id {propertyId} not found!");


            byte[]? propertyImage = null;

            // read image
            using var memoryStream = new MemoryStream();


            image.Image.CopyTo(memoryStream);
            propertyImage = memoryStream.ToArray();

            property.MainImage = propertyImage;

            await _unitOfWork.CompleteAsync();
        }

        public async Task UpdateAsync(int propertyId, UpdatedPropertDto propertyDto)
        {
            var prop = await _unitOfWork.Properties.GetAsync(p => p.Id == propertyId && p.Status != "rented");
            if (prop is null) throw new Exception($"Property with id : {propertyId} not found or not availble");


            prop.Title = propertyDto.Title;
            prop.Description = propertyDto.Description;
            prop.Location = propertyDto.Location;
            prop.Price = propertyDto.Price;

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

        

    }
}
