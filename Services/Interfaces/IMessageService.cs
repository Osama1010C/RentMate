using Microsoft.AspNetCore.Mvc;
using RentMateAPI.DTOModels.DTOMessage;

namespace RentMateAPI.Services.Interfaces
{
    public interface IMessageService
    {
        Task<List<SenderDto>> GetMyChatsAsync(int userId);
        Task<List<MessageDto>> GetChatContentAsync(int userId, int recieverId);
        Task AddMessageAsync(int senderId, int recieverId, string message);
    }
}
