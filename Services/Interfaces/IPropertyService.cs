using RentMateAPI.DTOModels.DTOImage;
using RentMateAPI.DTOModels.DTOProperty;

namespace RentMateAPI.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<List<AllPropertyDto>> GetAllAsync();
        Task<PropertyDto> GetDetailsAsync(int propertyId, int userId);
        Task<List<AllPropertyDto>> GetMyPropertiesAsync(int tenantId);

        //Task<int> AddAsync(AddPropertyDto propertyDto);
        Task<int> AddAsync(AddPropertyDto propertyDto, PropertyImagesDto imagesDto);
        //Task AddImageAsync(int propertyId, AddPropertyImageDto propertyImageDto);

        Task ReplaceMainImageAsync(int propertyId, ImageDto image);

        Task UpdateAsync(int propertyId, UpdatedPropertDto propertyDto);
        Task DeleteAsync(int propertyId);

        Task DeleteImageAsync(int propertyImageId);
    }
}
