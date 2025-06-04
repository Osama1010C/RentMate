using RentMateAPI.DTOModels.DTOHistory;

namespace RentMateAPI.Services.Interfaces
{
    public interface IHistoryService
    {
        Task<List<GetHistoryDto>> GetPropertiesHistoryAsync();
        Task<List<GetHistoryDto>> GetLandlordsRegistrationHistoryAsync();
        Task AddPropertyHistoryAsync(AddHistoryDto historyDto);
        Task AddLandlordsRegistrationHistoryAsync(AddHistoryDto historyDto);
    }
}
