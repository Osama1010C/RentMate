using Microsoft.Extensions.Logging;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTORent;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Services.Implementations
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        public RentalService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork; 
        }
        public async Task AcceptRequestAsync(int requestId)
        {
            var request = await _unitOfWork.RentalRequests.GetByIdAsync(requestId);


            if (request is null) throw new Exception("this rent request id not found");

            if (request.Status != "pending") throw new Exception("this request is already accepted or rejected");
            

            request.Status = "accepted";

            var property = await _unitOfWork.Properties.GetByIdAsync(request.PropertyId);

            property!.Status = "rented";

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
                garbage.Status = "rejected";

            await _unitOfWork.CompleteAsync();
        }

        
        public async Task RejectRequestAsync(int requestId)
        {
            var request = await _unitOfWork.RentalRequests.GetByIdAsync(requestId);


            if (request is null) throw new Exception("this rent request id not found");

            if (request.Status != "pending") throw new Exception("this request is already accepted or rejected");


            request.Status = "rejected";

            await _unitOfWork.CompleteAsync();
        }


        public async Task<List<RentPropertyRequestDto>> GetAllRequestsAsync(int landlordId)
        {
            var landlord = await _unitOfWork.Users.GetByIdAsync(landlordId);

            if (landlord == null) throw new Exception("this landlord id not found");


            var requests = await _unitOfWork.RentalRequests
                .GetAllAsync(r => (r.Property.LandlordId == landlordId) && (r.Status == "pending"), includeProperties: "Property,Tenant");

            if (requests == null) return new List<RentPropertyRequestDto>();

            var requestsInfo = requests.Select(r => new
            {
                r.Id,
                TenantName = r.Tenant.Name,
                PropertyTitle = r.Property.Title,
                PropertyMainImage = r.Property.MainImage,
                r.CreateAt,
                r.RequirmentDocument
            }).ToList();

            

            var result = requestsInfo.Select(r => new RentPropertyRequestDto
            {
                RentId = r.Id,
                TenantName = r.TenantName,
                PropertyTitle = r.PropertyTitle,
                PropertyMainImage = r.PropertyMainImage,
                CreateAt = r.CreateAt,
                RequirmentDocument = ConvertDocumentToList(r.RequirmentDocument)
            }).ToList();

            return result;
        }

        public async Task<List<TenantRentRequestDto>> GetTenantRequestsAsync(int tenantId)
        {
            var tenant = await _unitOfWork.Users.GetByIdAsync(tenantId);
            if (tenant is null) throw new Exception("this tenant id not found");


            //var requests = await _unitOfWork.RentalRequests
            //                .GetAllAsync(r => (r.TenantId == tenantId) && (r.Status == "pending"), includeProperties: "Property");
            var requests = await _unitOfWork.RentalRequests
                            .GetAllAsync(r => (r.TenantId == tenantId) , includeProperties: "Property");

            if (requests is null) return new List<TenantRentRequestDto>();

            var requestInfo = requests.Select(r => new TenantRentRequestDto
            {
                RentId = r.Id,
                TenantName = r.Tenant.Name,
                PropertyTitle = r.Property.Title,
                RentStatus = r.Status,
                PropertyMainImage = r.Property.MainImage,
                CreateAt = r.CreateAt,
            });

            return requestInfo.ToList();
        }

        public async Task RentPropertyAsync(RentPropertyDto rentDto)
        {
            if (!await IsExistAsync(rentDto.TenantId, rentDto.PropertyId))
                throw new Exception("this tenant or property id not found");
            
            if (await IsRequestedBeforeAsync(rentDto.TenantId, rentDto.PropertyId))
            {
                var request = await _unitOfWork.RentalRequests
                                    .GetAsync(r => r.TenantId == rentDto.TenantId && r.PropertyId == rentDto.PropertyId);

                if (request.Status == "accepted")
                    throw new Exception("You are already have this property!");

                if (request.Status == "pending")
                    _unitOfWork.RentalRequests.Delete(request.Id);
                //if (request.Status == "pending")
                //{
                //    _unitOfWork.RentalRequests.Delete(request.Id);
                //    await _unitOfWork.CompleteAsync();
                //    return;
                //}
            }

            var property = await _unitOfWork.Properties
                            .GetAsync(p => p.Id == rentDto.PropertyId);

            if (property is null) throw new Exception("this property id not found");

            if (property.Status == "rented" || property.PropertyApproval == "pending" || property.PropertyApproval == "rejected")
                throw new Exception("this property id is rented or not availble");

            if (rentDto.RequirmentDocument == null)
                throw new Exception("Please send the document!");

            byte[]? proposalDoc = null;
            using (var memoryStream = new MemoryStream())
            {
                rentDto.RequirmentDocument.CopyTo(memoryStream);
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

            await _unitOfWork.CompleteAsync();
        }


        public async Task CancelRentPropertyAsync(int tenantId, int propertyId)
        {
            if (!await IsExistAsync(tenantId, propertyId))
                throw new Exception("this tenant or property id not found");

            if (!await IsRequestedBeforeAsync(tenantId, propertyId))
                throw new Exception("this request is not found");

            var request = await _unitOfWork.RentalRequests
                                .GetAsync(r => r.TenantId == tenantId && r.PropertyId == propertyId && r.Status == "pending");

            if (request is null)
                throw new Exception("this request is not found");

            _unitOfWork.RentalRequests.Delete(request.Id);
            await _unitOfWork.CompleteAsync();
        }



        private List<string> ConvertDocumentToList(byte[] file)
        {
            var data = new List<string>();
            if (file == null) return data;

            using var memoryStream = new MemoryStream(file);
            using var reader = new StreamReader(memoryStream);
            string? line;
            while ((line = reader.ReadLine()) != null)
            {
                data.Add(line);
            }
            return data;
        }
        private async Task<bool> IsRequestedBeforeAsync(int tenantId, int propertyId)
        {
            var request =  await _unitOfWork.RentalRequests
                        .GetAllAsync(r => (r.TenantId == tenantId) && (r.PropertyId == propertyId) && ((r.Status == "rejected") || (r.Status == "pending")));
            
            return request.Count() > 0;
        }
        private async Task<bool> IsExistAsync(int tenantId, int propertyId)
        {
            bool isUserExist = await _unitOfWork.Users
                        .IsExistAsync(tenantId);
            bool isPropertyExist = await _unitOfWork.Properties
                        .IsExistAsync(propertyId);

            return isUserExist && isPropertyExist;

        }
    }
}
