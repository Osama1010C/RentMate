using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Services.Implementations
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;
        public SearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<AllPropertyDto>> SearchByLocationAsync(string location)
        {
            var properties = await _unitOfWork.Properties
                             .GetAllAsync(p => p.Location.Contains(location)
                                               &&
                                               p.Status == "available" && p.PropertyApproval == "accepted");
            var result = properties.Select(s => new AllPropertyDto
            {
                Id = s.Id,
                LandlordId = s.LandlordId,
                Title = s.Title,
                //Description = s.Description,
                Location = s.Location,
                Price = s.Price,
                Status = s.Status,
                Views = s.Views,
                MainImage = s.MainImage,
                //CreateAt = s.CreateAt,
            }).ToList();
            return result;
        }

        public async Task<List<AllPropertyDto>> SearchByPriceAsync(decimal fromPrice, decimal toPrice)
        {
            //handle greater number
            if (fromPrice > toPrice)
            {
                fromPrice = fromPrice + toPrice;
                toPrice = fromPrice - toPrice;
                fromPrice = fromPrice - toPrice;
            }

            var properties = await _unitOfWork.Properties
                            .GetAllAsync(p => (p.Price >= fromPrice && p.Price <= toPrice)
                                              && 
                                              p.Status == "available" && p.PropertyApproval == "accepted");

            var result = properties.Select(s => new AllPropertyDto
            {
                Id = s.Id,
                LandlordId = s.LandlordId,
                Title = s.Title,
                //Description = s.Description,
                Location = s.Location,
                Price = s.Price,
                Status = s.Status,
                Views = s.Views,
                MainImage = s.MainImage,
                //CreateAt = s.CreateAt,
            }).ToList();
            return result;
        }

        public async Task<List<PropertyDto>> SearchAsync(string location = null, decimal? fromPrice = null, decimal? toPrice = null)
        {
            // Ensure fromPrice is less than toPrice if both are provided
            if (fromPrice.HasValue && toPrice.HasValue && fromPrice > toPrice)
            {
                var temp = fromPrice;
                fromPrice = toPrice;
                toPrice = temp;
            }

            var properties = await _unitOfWork.Properties.GetAllAsync(p =>
                (string.IsNullOrEmpty(location) || p.Location.Contains(location)) &&
                (!fromPrice.HasValue || p.Price >= fromPrice.Value) &&
                (!toPrice.HasValue || p.Price <= toPrice.Value) &&
                p.Status == "available" 
                //p.PropertyApproval == "accepted"
                , includeProperties: "Landlord"
            );
            

            //var result = properties.Select(s => new AllPropertyDto
            //{
            //    Id = s.Id,
            //    LandlordId = s.LandlordId,
            //    Title = s.Title,
            //    Location = s.Location,
            //    Price = s.Price,
            //    Status = s.Status,
            //    Views = s.Views,
            //    MainImage = s.MainImage,
            //    PropertyApproval = s.PropertyApproval
            //}).ToList();

            //return result;

            var propertyDtos = properties.Select(p =>
            {
                //var landlordName = _unitOfWork.Users.GetByIdAsync(p.LandlordId).Result!.Name;
                return new PropertyDto
                {
                    Id = p.Id,
                    LandlordId = p.LandlordId,
                    LandlordName = p.Landlord!.Name,
                    Title = p.Title,
                    Description = p.Description,
                    Location = p.Location,
                    Price = p.Price,
                    Status = p.Status,
                    Views = p.Views,
                    MainImage = p.MainImage,
                    CreateAt = p.CreateAt,
                    PropertyImages = GetPropertyImagesAsync(p.Id).Result,
                    PropertyApproval = p.PropertyApproval
                };
            }).ToList();


            return propertyDtos;
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
