using Microsoft.EntityFrameworkCore.Metadata.Internal;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOImage;
using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Services.Implementations
{
    public class PropertyService : IPropertyService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PropertyService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        

        public async Task<List<AllPropertyDto>> GetAllAsync()
        {
            var properties = await _unitOfWork.Properties.GetAllAsync(includeProperties:"Landlord");

            var propertyDtos = properties.Select(property => new AllPropertyDto
            {
                Id = property.Id,
                //LandlordId = property.LandlordId,
                //LandlordName = property.Landlord!.Name,
                Title = property.Title,
                //Description = property.Description,
                Location = property.Location,
                Price = property.Price,
                Status = property.Status,
                Views = property.Views,
                MainImage = property.MainImage,
                //CreateAt = property.CreateAt,
                PropertyApproval = property.PropertyApproval
            }).ToList();

            return propertyDtos;
        }

        public async Task<PropertyDto> GetDetailsAsync(int propertyId, int userId)
        {
            if(await _unitOfWork.Users.GetByIdAsync(userId) == null) 
                throw new Exception($"User with Id {userId} not found!");

            var property = await _unitOfWork.Properties.GetAsync(p => p.Id == propertyId && p.PropertyApproval == "accepted", includeProperties:"Landlord");
            if (property is null) 
                throw new Exception($"Property with Id {propertyId} not exist!");

            var propertyDto = new PropertyDto
            {
                Id = property.Id,
                LandlordId = property.LandlordId,
                LandlordName = property.Landlord!.Name,
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

        public async Task<List<AllPropertyDto>> GetMyPropertiesAsync(int tenantId)
        {
            if (await _unitOfWork.Users.GetByIdAsync(tenantId) == null)
                throw new Exception($"User with Id {tenantId} not found!");


            var properties = await _unitOfWork.TenantProperties.GetAllAsync(p => p.TenantId == tenantId, includeProperties:"Property");


            var propertyDtos = properties.Select(p =>
                {
                    var landlordName = _unitOfWork.Users.GetByIdAsync(p.Property.LandlordId).Result!.Name;
                    return new AllPropertyDto
                    {
                        Id = p.Property.Id,
                        //LandlordId = p.Property.LandlordId,
                        //LandlordName = landlordName,
                        Title = p.Property.Title,
                        //Description = p.Property.Description,
                        Location = p.Property.Location,
                        Price = p.Property.Price,
                        Status = p.Property.Status,
                        Views = p.Property.Views,
                        MainImage = p.Property.MainImage,
                        //CreateAt = p.Property.CreateAt,
                        PropertyApproval = p.Property.PropertyApproval
                    };
                }).ToList();
            

            return propertyDtos;
        }

        public async Task AddAsync(AddPropertyDto propertyDto)
        {
            if(await _unitOfWork.Users.GetByIdAsync(propertyDto.LandlordId) is null)
                throw new Exception($"Landlord with Id {propertyDto.LandlordId} not found!");

            byte[]? propertyImage = null;

            // read main image
            using (var memoryStream = new MemoryStream())
            {
                propertyDto.MainImage.CopyTo(memoryStream);
                propertyImage = memoryStream.ToArray();

            }


            await _unitOfWork.Properties.AddAsync(new()
            {
                LandlordId = propertyDto.LandlordId,
                Title = propertyDto.Title,
                Description = propertyDto.Description,
                Location = propertyDto.Location,
                Price = propertyDto.Price,
                MainImage = propertyImage,
                Views = 0,
                Status = "available",
                PropertyApproval = "pending"
            });

            await _unitOfWork.CompleteAsync();
        }

        public async Task AddImageAsync(int propertyId, AddPropertyImageDto propertyImageDto)
        {
        

            if (await _unitOfWork.Properties.GetByIdAsync(propertyId) is null)
                throw new Exception($"Property with Id {propertyId} not found!");


            byte[]? propertyImage = null;

            // read image
            using var memoryStream = new MemoryStream();


            propertyImageDto.Image!.CopyTo(memoryStream);
            propertyImage = memoryStream.ToArray();

            await _unitOfWork.PropertyImages.AddAsync(new()
            {
                PropertyId = propertyId,
                Image = propertyImage,
            });

            await _unitOfWork.CompleteAsync();
        }

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
