using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Helpers
{
    public static class SavePostHelper
    {
        public static async Task<bool> IsNewSavedPostAsync(IUnitOfWork unitOfWork, int tenantId, int propertyId)
        {
            var savedPost = await unitOfWork.SavedPosts
                        .GetAllAsync(p => p.TenantId == tenantId && p.PropertyId == propertyId);

            return savedPost.Count() == 0 ? true : false;
        }
    }
}
