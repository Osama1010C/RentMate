using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.Helpers;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using RentMateAPI.Validations.Interfaces;

namespace RentMateAPI.Services.Implementations
{
    public class SavedPostService : ISavedPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IModelValidator<User> _userValidator;
        private readonly IModelValidator<Property> _propertyValidator;
        public SavedPostService(IUnitOfWork unitOfWork, IModelValidator<User> userValidator, IModelValidator<Property> propertyValidator)
        {
            this._unitOfWork = unitOfWork;
            this._userValidator = userValidator;
            this._propertyValidator = propertyValidator;
        }
        public async Task<List<PropertyDto>> GetAllSavedAsync(int tenantId)
        {
            var tenant = await _userValidator.IsModelExistReturn(tenantId);

            var savedPosts = await _unitOfWork.SavedPosts.GetAllAsync(sp => sp.TenantId == tenantId, includeProperties: "Property");

            var propertyDtos = savedPosts.Select(p =>
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
                    PropertyImages = PropertyImageHelper.GetPropertyImagesAsync(_unitOfWork,p.PropertyId).Result,
                    PropertyApproval = p.Property.PropertyApproval
                };
            }).OrderByDescending(p => p.Views).ToList();

            return propertyDtos;
        }

        public async Task SavePostAsync(int tenantId, int propertyId)
        {
            await _userValidator.IsModelExist(tenantId);
            await _propertyValidator.IsModelExist(propertyId);

            if (await SavePostHelper.IsNewSavedPostAsync(_unitOfWork, tenantId, propertyId))
            {
                await _unitOfWork.SavedPosts.AddAsync(new()
                {
                    TenantId = tenantId,
                    PropertyId = propertyId
                });
            }
            else
            {
                var savedProperty = await _unitOfWork.SavedPosts
                    .GetAsync(p => (p.TenantId == tenantId) && (p.PropertyId == propertyId));
                _unitOfWork.SavedPosts.Delete(savedProperty.Id);
            }
            await _unitOfWork.CompleteAsync();
        }
    }
}
