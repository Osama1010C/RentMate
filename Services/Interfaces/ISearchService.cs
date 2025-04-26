using RentMateAPI.DTOModels.DTOProperty;
namespace RentMateAPI.Services.Interfaces
{
    public interface ISearchService
    {
        Task<List<AllPropertyDto>> SearchByPriceAsync(decimal fromPrice, decimal toPrice);
        Task<List<AllPropertyDto>> SearchByLocationAsync(string location);
    }
}
