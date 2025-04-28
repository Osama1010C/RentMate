using RentMateAPI.DTOModels.DTOProperty;

namespace RentMateAPI.Services.Interfaces
{
    public interface ISavedPostService
    {
        Task<List<PropertyDto>> GetAllSavedAsync(int tenantId);

        Task SavePostAsync(int tenantId, int propertyId);
    }
}
