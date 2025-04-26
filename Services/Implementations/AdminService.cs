using RentMateAPI.DTOModels.DTODashboard;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        public AdminService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }
        public async Task AcceptLandlordRegistrationAsync(int requestId)
        {
            var request = await _unitOfWork.PendingLandlord.GetByIdAsync(requestId);

            if (request is null) throw new Exception($"Request id : {requestId} not found");

            _unitOfWork.PendingLandlord.Delete(request.Id);

            await _unitOfWork.Users.AddAsync(new()
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Image = request.Image,
                Role = "landlord"
            });

            await _unitOfWork.CompleteAsync();
        }

        public async Task RejectLandlordRegistrationAsync(int requestId)
        {
            var request = await _unitOfWork.PendingLandlord.GetByIdAsync(requestId);

            if (request is null) throw new Exception($"Request id : {requestId} not found");

            _unitOfWork.PendingLandlord.Delete(request.Id);

            await _unitOfWork.CompleteAsync();
        }

        public async Task AcceptPropertyPostAsync(int propertyId)
        {
            var post = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            
            if (post is null || post.PropertyApproval != "pending") throw new Exception($"Property id : {propertyId} not found");

            post.PropertyApproval = "accepted";

            await _unitOfWork.CompleteAsync();
        }

        

        public async Task RejectPropertyPostAsync(int propertyId)
        {
            var post = await _unitOfWork.Properties.GetByIdAsync(propertyId);

            if (post is null || post.PropertyApproval != "pending") throw new Exception($"Property id : {propertyId} not found");

            _unitOfWork.Properties.Delete(post.Id);

            await _unitOfWork.CompleteAsync();
        }

        public async Task<DashboardDto> GetStatisticsAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();
            var numOfUsers = users.Count();
            var numOfAdmins = users.Count(u => u.Role == "admin");
            var numOfTenants = users.Count(u => u.Role == "tenant");
            var numOfLandlords = users.Count(u => u.Role == "landlord");

            var pendingLandlordsList = await _unitOfWork.PendingLandlord.GetAllAsync();
            var numOfpendingLandlords = pendingLandlordsList.Count();

            var properties = await _unitOfWork.Properties.GetAllAsync();
            var numOfProperties = properties.Count(p => p.PropertyApproval == "accepted");
            var numOfPendingProperties = properties.Count(p => p.PropertyApproval == "pending");

            
            var dashBoard = new DashboardDto
            {
                NumberOfUsers = numOfUsers,
                NumberOfAdmins = numOfAdmins,
                NumberOfTenants = numOfTenants,
                NumberOfLandlords = numOfLandlords,
                NumberOfPendingLandlordRegistrations = numOfpendingLandlords,
                NumberOfProperties = numOfProperties,
                NumberOfPendingProperties = numOfPendingProperties
            };

            return dashBoard;
        }
    }
}
