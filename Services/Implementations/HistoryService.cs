using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOHistory;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Services.Implementations
{
    public class HistoryService : IHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        public HistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task AddLandlordsRegistrationHistoryAsync(AddHistoryDto historyDto)
        {
            await _unitOfWork.Histories.AddAsync(new History
            {
                UserId = historyDto.UserId,
                Description = historyDto.Description,
                HistoryType = historyDto.HistoryType
            });
        }

        public async Task AddPropertyHistoryAsync(AddHistoryDto historyDto)
        {
            await _unitOfWork.Histories.AddAsync(new History
            {
                UserId = historyDto.UserId,
                Description = historyDto.Description,
                HistoryType = historyDto.HistoryType
            });
        }

        public async Task<List<GetHistoryDto>> GetLandlordsRegistrationHistoryAsync()
        {
            var history = await _unitOfWork.Histories.GetAllAsync(h => h.HistoryType == "Registration Action");

            return history.Select(h => new GetHistoryDto
            {
                Description = h.Description,
                HistoryType = h.HistoryType,
                ActionDate = h.ActionDate,
            })
                .OrderByDescending(h => h.ActionDate)
                .ToList();
        }

        public async Task<List<GetHistoryDto>> GetPropertiesHistoryAsync()
        {
            var history = await _unitOfWork.Histories.GetAllAsync(h => h.HistoryType == "Property Action");

            return history.Select(h => new GetHistoryDto
            {
                Description = h.Description,
                HistoryType = h.HistoryType
            })
                .OrderByDescending(h => h.ActionDate)
                .ToList();
        }


    }
}
