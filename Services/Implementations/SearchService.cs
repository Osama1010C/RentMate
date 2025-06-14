﻿using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.Helpers;
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
                , includeProperties: "Landlord"
            );




            var propertyDtos = properties.Select(p =>
            {
                return new PropertyDto
                {
                    Id = p.Id,
                    LandlordId = p.LandlordId,
                    LandlordName = p.Landlord!.Name,
                    LandlordImage = p.Landlord.Image,
                    Title = p.Title,
                    Description = p.Description,
                    Location = p.Location,
                    Price = p.Price,
                    Status = p.Status,
                    Views = p.Views,
                    MainImage = p.MainImage,
                    CreateAt = p.CreateAt,
                    PropertyImages = PropertyImageHelper.GetPropertyImagesAsync(_unitOfWork, p.Id).Result,
                    PropertyApproval = p.PropertyApproval
                };
            }).OrderByDescending(p => p.Views).ToList();


            return propertyDtos;
        }
    }
}
