using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTODashboard;
using RentMateAPI.DTOModels.DTOHistory;
using RentMateAPI.DTOModels.DTONotification;
using RentMateAPI.Helpers;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using RentMateAPI.Validations.Interfaces;

namespace RentMateAPI.Services.Implementations
{
    public class AdminService : IAdminService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHistoryService _historyService;
        private readonly INotificationService _notificationService;
        private readonly IModelValidator<PendingLandlord> _pendingLandlordValidator;
        public AdminService(IUnitOfWork unitOfWork, IHistoryService historyService, INotificationService notificationService,
            IModelValidator<PendingLandlord> pendingLandlordValidator)
        {
            this._unitOfWork = unitOfWork;
            _historyService = historyService;
            _notificationService = notificationService;
            _pendingLandlordValidator = pendingLandlordValidator;
        }
        public async Task AcceptLandlordRegistrationAsync(int requestId)
        {
            var request = await _pendingLandlordValidator.IsModelExistReturn(requestId);

            _unitOfWork.PendingLandlord.Delete(request.Id);

            await _unitOfWork.Users.AddAsync(new()
            {
                Name = request.Name,
                Email = request.Email,
                Password = request.Password,
                Image = request.Image,
                Role = "landlord"
            });
            await _historyService.AddLandlordsRegistrationHistoryAsync(new AddHistoryDto
            {
                UserId = AdminHelper.GetAdminId(_unitOfWork).Result,
                Description = $"{request.Name}'s registration Landlord request has been Accepted",
                HistoryType = "Registration Action"
            });

            await _unitOfWork.CompleteAsync();
        }

        public async Task RejectLandlordRegistrationAsync(int requestId)
        {
            var request = await _pendingLandlordValidator.IsModelExistReturn(requestId);


            await _historyService.AddLandlordsRegistrationHistoryAsync(new AddHistoryDto
            {
                UserId = AdminHelper.GetAdminId(_unitOfWork).Result,
                Description = $"{request.Name}'s registration Landlord request has been Rejected",
                HistoryType = "Registration Action"
            });

            _unitOfWork.PendingLandlord.Delete(request.Id);

            await _unitOfWork.CompleteAsync();
        }

        public async Task AcceptPropertyPostAsync(int propertyId)
        {
            var post = await _unitOfWork.Properties.GetByIdAsync(propertyId);
            
            if (post is null || post.PropertyApproval != "pending") throw new Exception($"Property id : {propertyId} not found");

            post.PropertyApproval = "accepted";
            await _historyService.AddPropertyHistoryAsync(new AddHistoryDto
            {
                UserId = AdminHelper.GetAdminId(_unitOfWork).Result,
                Description = $"Property post {post.Title} has been Accepted",
                HistoryType = "Property Action",
            });

            await _notificationService.AddNotificationAsync((int)post.LandlordId!, new AddNotificationDto
            {
                Description = $"Your property post {post.Title} has been Accepted",
                NotificationType = "Property Action",
                NotificationTypeId = post.Id
            });

            await _unitOfWork.CompleteAsync();
        }

        

        public async Task RejectPropertyPostAsync(int propertyId)
        {
            var post = await _unitOfWork.Properties.GetByIdAsync(propertyId);

            if (post is null || post.PropertyApproval != "pending") throw new Exception($"Property id : {propertyId} not found");

            await _historyService.AddPropertyHistoryAsync(new AddHistoryDto
            {
                UserId = AdminHelper.GetAdminId(_unitOfWork).Result,
                Description = $"Property post {post.Title} has been Rejected",
                HistoryType = "Property Action"
            });

            _unitOfWork.Properties.Delete(post.Id);

            await _notificationService.AddNotificationAsync((int)post.LandlordId!, new AddNotificationDto
            {
                Description = $"Your property post {post.Title} has been Rejected",
                NotificationType = "Property Action",
                NotificationTypeId = post.Id
            });

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
