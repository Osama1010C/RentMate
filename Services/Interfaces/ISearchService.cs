using RentMateAPI.DTOModels.DTOProperty;
namespace RentMateAPI.Services.Interfaces
{
    public interface ISearchService
    {
        Task<List<AllPropertyDto>> SearchByPriceAsync(decimal fromPrice, decimal toPrice);
        Task<List<AllPropertyDto>> SearchByLocationAsync(string location);
        Task<List<AllPropertyDto>> SearchAsync(string? location = null, decimal? fromPrice = null, decimal? toPrice = null);
    }
}
