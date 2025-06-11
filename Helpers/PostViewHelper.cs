using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Helpers
{
    public static class PostViewHelper
    {
        public static async Task<bool> IsNewViewAsync(IUnitOfWork unitOfWork, int userId, int propertyId)
        {
            var propertyView = await unitOfWork.PropertyViews.GetAsync(p => p.UserId == userId && p.PropertyId == propertyId);

            return propertyView is null;
        }
    }
}
