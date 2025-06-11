using RentMateAPI.Data.Models;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Services.Implementations
{
    public class PendingLandlordService : IPendingLandlordService
    {
        private readonly IUnitOfWork _unitOfWork;
        public PendingLandlordService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> AddAsync(PendingLandlord landlord)
        {
            bool isAlreadyExist = false;
            bool isAlreadyExistInPendings = false;
            var hashedPassword = BC.EnhancedHashPassword(landlord.Password);


            var users = await _unitOfWork.Users.GetAllAsync();
            var pendings = await _unitOfWork.PendingLandlord.GetAllAsync();

            isAlreadyExist = users.Any(u => u.Name == landlord.Name || u.Email == landlord.Email || BC.EnhancedVerify(landlord.Password, u.Password));

            isAlreadyExistInPendings = pendings.Any(p => p.Name == landlord.Name || p.Email == landlord.Email || BC.EnhancedVerify(landlord.Password, p.Password));

            if (isAlreadyExist || isAlreadyExistInPendings) return false;

            landlord.Password = hashedPassword;
            await _unitOfWork.PendingLandlord.AddAsync(landlord);
            return await _unitOfWork.CompleteAsync() > 0;
        }

        public async Task<List<PendingLandlord>> GetAllAsync() => await _unitOfWork.PendingLandlord.GetAllAsync();

    }
}
