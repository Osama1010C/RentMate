using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Services.Implementations
{
    public class SavedPostService : ISavedPostService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SavedPostService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task<List<PropertyDto>> GetAllSavedAsync(int tenantId)
        {
            
            var tenant = await _unitOfWork.Users.GetByIdAsync(tenantId);

            if (tenant is null) throw new Exception("this tenant id not found");

            

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
                    PropertyImages = GetPropertyImagesAsync(p.Property.Id).Result,
                    PropertyApproval = p.Property.PropertyApproval
                };
            }).OrderByDescending(p => p.Views).ToList();


            return propertyDtos;

        }

        public async Task SavePostAsync(int tenantId, int propertyId)
        {
            if (!await IsExistAsync(tenantId, propertyId))
                throw new Exception("this user or property id not found");

            if (await IsNewSavedPostAsync(tenantId, propertyId))
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


        private async Task<bool> IsNewSavedPostAsync(int tenantId, int propertyId)
        {
            var savedPost = await _unitOfWork.SavedPosts
                        .GetAllAsync(p => (p.TenantId == tenantId) && (p.PropertyId == propertyId));

            return savedPost.Count() == 0 ? true : false;
        }

        private async Task<bool> IsExistAsync(int tenantId, int propertyId)
        {
            bool isUserExist = await _unitOfWork.Users.IsExistAsync(tenantId);
            bool isPropertyExist = await _unitOfWork.Properties.IsExistAsync(propertyId);

            return isUserExist && isPropertyExist;

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
