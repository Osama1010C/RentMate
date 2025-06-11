using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTONotification;
using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.DTOModels.DTORent;
using RentMateAPI.Helpers;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using RentMateAPI.Validations.Interfaces;

namespace RentMateAPI.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;
        private readonly IFileValidator _fileValidator;
        private readonly IModelValidator<User> _userValidator;
        private readonly IModelValidator<Property> _propertyValidator;
        private readonly IModelValidator<RentalRequest> _requestValidator;
        public RentalService(IUnitOfWork unitOfWork, INotificationService notificationService, IFileValidator fileValidator,
            IModelValidator<User> userValidator, IModelValidator<Property> propertyValidator, IModelValidator<RentalRequest> requestValidator)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
            _fileValidator = fileValidator;
            _userValidator = userValidator;
            _propertyValidator = propertyValidator;
            _requestValidator = requestValidator;
        }
        public async Task AcceptRequestAsync(int requestId)
        {
            var request = await _requestValidator.IsModelExistReturn(requestId);


            if (request.Status != "pending") throw new Exception("this request is already accepted or rejected");
            

            request.Status = "accepted";

            var property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId);

            property!.Status = "rented";

            var notificationDto = new AddNotificationDto
            {
                Description = $"Your request for the property '{property.Title}' has been Accepted",
                NotificationType = "Rent Request Action",
                NotificationTypeId = property.Id
            };
            await _notificationService.AddNotificationAsync(request.TenantId, notificationDto);

            // add this property to tenantProperty table
            var tenantProperty = new TenantProperty
            {
                PropertyId = property.Id,
                TenantId = request.TenantId,
            };
            await _unitOfWork.TenantProperties.AddAsync(tenantProperty);

            // reject all requests that ask for the same property
            var rentalRequests = await _unitOfWork.RentalRequests.GetAllAsync();
            var garbageRequests = rentalRequests.Where(x => x.Id != requestId && x.PropertyId == property.Id).ToList();

            foreach (var garbage in garbageRequests)
            {
                garbage.Status = "rejected";
                var notificationRejectDto = new AddNotificationDto
                {
                    Description = $"Your request for the property '{property.Title}' has been Rejected",
                    NotificationType = "Rent Request Action",
                    NotificationTypeId = property.Id
                };
                await _notificationService.AddNotificationAsync(garbage.TenantId, notificationRejectDto);
            }
                

            await _unitOfWork.CompleteAsync();
        }

        
        public async Task RejectRequestAsync(int requestId)
        {
            var request = await _requestValidator.IsModelExistReturn(requestId);

            if (request.Status != "pending") throw new Exception("this request is already accepted or rejected");


            request.Status = "rejected";

            var property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId);


            var notificationRejectDto = new AddNotificationDto
            {
                Description = $"Your request for the property '{property!.Title}' has been Rejected",
                NotificationType = "Rent Request Action",
                NotificationTypeId = property.Id
            };
            await _notificationService.AddNotificationAsync(request.TenantId, notificationRejectDto);

            await _unitOfWork.CompleteAsync();
        }


        public async Task<List<RentPropertyRequestDto>> GetAllRequestsAsync(int landlordId)
        {
            var landlord = await _userValidator.IsModelExistReturn(landlordId);

            var requests = await _unitOfWork.RentalRequests
                .GetAllAsync(r => (r.Property.LandlordId == landlordId) && (r.Status == "pending"), includeProperties: "Property,Tenant");

            if (requests == null) return new List<RentPropertyRequestDto>();

            var result = requests.Select(r => new RentPropertyRequestDto
            {
                RentId = r.Id,
                TenantName = r.Tenant.Name,
                TenantImage = r.Tenant.Image,
                PropertyTitle = r.Property.Title,
                PropertyMainImage = r.Property.MainImage,
                CreateAt = r.CreateAt,
                RequirmentDocument = DocumentHelper.ConvertDocumentToList(r.RequirmentDocument)
            }).ToList();

            return result;
        }

        public async Task<List<RentPropertyRequestDto>> GetAllRequestsAsync(int landlordId, int propertyId)
        {
            await _propertyValidator.IsModelExist(propertyId);

            var landlord = await _userValidator.IsModelExistReturn(landlordId);

            var requests = await _unitOfWork.RentalRequests
                .GetAllAsync(r => r.PropertyId == propertyId && (r.Property.LandlordId == landlordId) && (r.Status == "pending"), includeProperties: "Property,Tenant");

            if (requests == null) return new List<RentPropertyRequestDto>();


            var result = requests.Select(r => new RentPropertyRequestDto
            {
                RentId = r.Id,
                TenantName = r.Tenant.Name,
                TenantImage = r.Tenant.Image,
                PropertyTitle = r.Property.Title,
                PropertyMainImage = r.Property.MainImage,
                CreateAt = r.CreateAt,
                RequirmentDocument = DocumentHelper.ConvertDocumentToList(r.RequirmentDocument)
            }).ToList();

            return result;
        }

        public async Task<List<PropertyRequestDto>> GetTenantRequestsAsync(int tenantId)
        {
            var tenant = await _userValidator.IsModelExistReturn(tenantId);
            
            var requests = await _unitOfWork.RentalRequests
                            .GetAllAsync(r => (r.TenantId == tenantId) , includeProperties: "Property");

            if (requests is null) return new List<PropertyRequestDto>();

            
            var requestInfo = requests.Select(r =>
            {
                var landlordName = _unitOfWork.Users.GetByIdAsync(r.Property.LandlordId).Result!.Name;
                var landlordImage = _unitOfWork.Users.GetByIdAsync(r.Property.LandlordId).Result!.Image;
                return new PropertyRequestDto
                {
                    PropertyId = r.Property.Id,
                    RentId = r.Id,
                    TenantName = r.Tenant.Name,
                    PropertyTitle = r.Property.Title,
                    RentStatus = r.Status,
                    PropertyMainImage = r.Property.MainImage,
                    RequestCreateAt = r.CreateAt,    
                    LandlordId = r.Property.LandlordId,
                    LandlordName = landlordName,
                    LandlordImage = landlordImage,
                    Title = r.Property.Title,
                    Description = r.Property.Description,
                    Location = r.Property.Location,
                    Price = r.Property.Price,
                    Status = r.Property.Status,
                    Views = r.Property.Views,
                    PropertyImages = PropertyImageHelper.GetPropertyImagesAsync(_unitOfWork, r.Property.Id).Result,
                    PropertyCreateAt = r.Property.CreateAt
                };
            });

            return requestInfo.ToList();
        }

        public async Task RentPropertyAsync(RentPropertyDto rentDto)
        {
            _fileValidator.IsNullFile(rentDto.RequirmentDocument!);
            _fileValidator.IsValidFileExtension(rentDto.RequirmentDocument!, ".txt");
            _fileValidator.IsValidFileSize(rentDto.RequirmentDocument!, 200); // 200 B

            await _userValidator.IsModelExist(rentDto.TenantId);
            await _propertyValidator.IsModelExist(rentDto.PropertyId);

            if (await RentalHelper.IsRequestedBeforeAsync(_unitOfWork, rentDto.TenantId, rentDto.PropertyId))
            {
                var request = await _unitOfWork.RentalRequests
                                    .GetAsync(r => r.TenantId == rentDto.TenantId && r.PropertyId == rentDto.PropertyId);

                if (request.Status == "accepted")
                    throw new Exception("You are already have this property!");

                if (request.Status == "pending")
                    _unitOfWork.RentalRequests.Delete(request.Id);
                
            }

            
            var property = await _propertyValidator.IsModelExistReturn(rentDto.PropertyId);


            if (property.Status == "rented" || property.PropertyApproval == "pending" || property.PropertyApproval == "rejected")
                throw new Exception("this property id is rented or not availble");


            byte[]? proposalDoc = null;
            using (var memoryStream = new MemoryStream())
            {
                rentDto.RequirmentDocument!.CopyTo(memoryStream);
                proposalDoc = memoryStream.ToArray();
            }
           
            await _unitOfWork.RentalRequests.AddAsync(new()
            {
                TenantId = rentDto.TenantId,
                PropertyId = rentDto.PropertyId,
                RequirmentDocument = proposalDoc,
                CreateAt = DateTime.Now,
                Status = "pending"
            });
            var notificationDto = new AddNotificationDto
            {
                Description = $"A new rental request for your property '{property.Title}'",
                NotificationType = "Rent Request",
                NotificationTypeId = property.Id
            };
            await _notificationService.AddNotificationAsync((int)property.LandlordId!, notificationDto);

            await _unitOfWork.CompleteAsync();
        }


        public async Task CancelRentPropertyAsync(int tenantId, int propertyId)
        {
            await _userValidator.IsModelExist(tenantId);
            await _propertyValidator.IsModelExist(propertyId);

            if (!await RentalHelper.IsRequestedBeforeAsync(_unitOfWork, tenantId, propertyId))
                throw new Exception("this request is not found");

            var request = await _unitOfWork.RentalRequests
                                .GetAsync(r => r.TenantId == tenantId && r.PropertyId == propertyId && r.Status == "pending");

            if (request is null)
                throw new Exception("this request is not found");


            _unitOfWork.RentalRequests.Delete(request.Id);

            
            var property = await _propertyValidator.IsModelExistReturn(propertyId);

            var notification = await _unitOfWork.Notifications.GetAsync(n => n.UserId == property.LandlordId &&
                                n.Description.Equals($"A new rental request for your property '{property.Title}'"));
            if (notification is null)
                throw new Exception("this notification is not found");

            _unitOfWork.Notifications.Delete(notification.Id);

            await _unitOfWork.CompleteAsync();
        }
    }
}
