using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Helpers
{
    public static class AdminHelper
    {
        public static async Task<int> GetAdminId(IUnitOfWork unitOfWork)
        {
            var admin = await unitOfWork.Users.GetAllAsync(u => u.Role == "admin");
            return admin[0].Id;
        }
    }
}
