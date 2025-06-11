using RentMateAPI.DTOModels.DTOProperty;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Helpers
{
    public static class PropertyImageHelper
    {
        public static async Task<List<PropertyImageDto>> GetPropertyImagesAsync(IUnitOfWork unitOfWork, int propertyId)
        {
            var images = await unitOfWork.PropertyImages.GetAllAsync(p => p.PropertyId == propertyId);
            return images.Select(m => new PropertyImageDto
            {
                PropertyImageId = m.Id,
                Image = m.Image
            }).ToList();
        }
    }

}
