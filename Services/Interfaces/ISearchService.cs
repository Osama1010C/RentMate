using RentMateAPI.DTOModels.DTOProperty;
namespace RentMateAPI.Services.Interfaces
{
    public interface ISearchService
    {
        Task<List<PropertyDto>> SearchAsync(string? location = null, decimal? fromPrice = null, decimal? toPrice = null);
    }
}
