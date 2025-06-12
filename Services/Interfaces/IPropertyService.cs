using RentMateAPI.DTOModels.DTOImage;
using RentMateAPI.DTOModels.DTOProperty;

namespace RentMateAPI.Services.Interfaces
{
    public interface IPropertyService
    {
        Task<int> GetNumberOfPages();
        Task<List<PropertyDto>> GetAllAsync();
        Task<List<PropertyDto>> GetPageAsync(int pageNumber);
        Task<PropertyDetailsDto> GetDetailsAsync(int propertyId, int userId);
        Task<List<PropertyDto>> GetMyPropertiesAsync(int tenantId);

        Task<int> AddAsync(AddPropertyDto propertyDto, PropertyImagesDto imagesDto);
        Task AddImageAsync(int propertyId, AddPropertyImageDto propertyImageDto);

        Task UpdatePropertyAsync(int propertyId, UpdatedPropertDto propertyDto, ImageDto? image = null);
        Task DeleteAsync(int propertyId);

        Task DeleteImageAsync(int propertyImageId);
    }
}
