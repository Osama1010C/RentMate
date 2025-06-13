using Microsoft.AspNetCore.DataProtection;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Helpers
{
    public static class MessageHelper
    {
        public static async Task<string> GetLastMessage(IUnitOfWork unitOfWork, int senderId, int receiverId)
        {
            var chat = await unitOfWork.Messages
                .GetAllAsync(m => (m.SenderId == senderId && m.ReceiverId == receiverId) || (m.SenderId == receiverId && m.ReceiverId == senderId),
                orderBy: m => m.OrderByDescending(m => m.SentAt));

            return chat.FirstOrDefault()!.Content;
        }
        public static string ShapeLargeMessgae(string message)
           => message.Length > 50 ? message.Substring(0, 30) + "..." : message;
    }
}
