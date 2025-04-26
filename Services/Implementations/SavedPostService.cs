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
        public async Task<List<AllPropertyDto>> GetAllSavedAsync(int tenantId)
        {
            
            var tenant = await _unitOfWork.Users.GetByIdAsync(tenantId);

            if (tenant is null) throw new Exception("this tenant id not found");

            

            var savedPosts = await _unitOfWork.SavedPosts.GetAllAsync(sp => sp.TenantId == tenantId, includeProperties: "Property");



            var propertyDtos = savedPosts.Select(sp => new AllPropertyDto
            {
                Id = sp.Property.Id,
                //LandlordId = sp.Property.LandlordId,
                Title = sp.Property.Title,
                //Description = sp.Property.Description,
                Location = sp.Property.Location,
                Price = sp.Property.Price,
                Status = sp.Property.Status,
                Views = sp.Property.Views,
                MainImage = sp.Property.MainImage,
                //CreateAt = sp.Property.CreateAt,
            }).ToList();

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

                await _unitOfWork.CompleteAsync();
            }
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
        
    }
}
