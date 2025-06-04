using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOMessage;
using RentMateAPI.DTOModels.DTONotification;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

// data protection is used to encrypt and decrypt the message content.

//Encryption   =>	AES-256 with HMACSHA256 for integrity

//algorithm : AES-256-CBC
//------------------------
//Symmetric encryption(same key for encrypt and decrypt).
//256 bits = very strong encryption.


//HMACSHA256
//------------------
//After encrypting, it signs the encrypted data to ensure nobody tampered with it.

//How HMACSHA256 works:

//It takes:

//The encrypted data (ciphertext)
//A secret key
//It runs a hashing algorithm (SHA-256) over this data plus the key.


namespace RentMateAPI.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IDataProtector _dataProtector;
        private readonly INotificationService _notificationService;
        public MessageService(IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext,
            IDataProtectionProvider dataProtectionProvider, IConfiguration configuration,
            INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _dataProtector = dataProtectionProvider.CreateProtector(configuration["SecretKey"]!);
            _notificationService = notificationService;
        }

        public async Task AddMessageAsync(int senderId, int recieverId, string message)
        {
            if (!await IsExistAsync(senderId, recieverId))
                throw new Exception("this user id not found");

            var encryptedMessage = _dataProtector.Protect(message);

            var msg = new Message
            {
                SenderId = senderId,
                ReceiverId = recieverId,
                Content = encryptedMessage,
            };


            await _unitOfWork.Messages.AddAsync(msg);
            await _unitOfWork.CompleteAsync();

            var sender = await _unitOfWork.Users.GetByIdAsync(senderId);

            // Prepare message payload
            var messageDto = new MessageDto
            {
                SenderName = sender!.Name,
                SentAt = msg.SentAt,
                Content = message
            };


            if (ChatHub.UserConnections.TryGetValue(recieverId, out string connectionId))
            {
                await _hubContext.Clients.Client(connectionId).SendAsync("ReceiveMessage", messageDto);
            }

            var IsMessagedBefore = await IsMessagedBeforeInNotification(messageDto.SenderName, recieverId);
            if (!IsMessagedBefore)
            {
                var notificationDto = new AddNotificationDto
                {
                    Description = $"{messageDto.SenderName} sent you a message",
                    NotificationType = "Message",
                    NotificationTypeId = msg.SenderId
                };
                await _notificationService.AddNotificationAsync(recieverId, notificationDto);

            }
        }

        public async Task<List<MessageDto>> GetChatContentAsync(int userId, int recieverId)
        {
            if (!await IsExistAsync(userId, recieverId))
                throw new Exception("this user id not found");


            var chat = await _unitOfWork.Messages
                              .GetAllAsync(m => (m.SenderId == userId || m.ReceiverId == userId) && (m.SenderId == recieverId || m.ReceiverId == recieverId),
                              includeProperties: "Sender,Receiver",
                              orderBy: m => m.OrderBy(m => m.SentAt));


            var messages = chat.Select(c => new MessageDto
            {
                SenderName = c.Sender.Name,
                SentAt = c.SentAt,
                Content = _dataProtector.Unprotect(c.Content),
            })
            .ToList();

            return messages;
        }

        public async Task<List<SenderDto>> GetMyChatsAsync(int userId)
        {

            if (!await _unitOfWork.Users.IsExistAsync(userId))
                throw new Exception("this user id not found");


            var chats = await _unitOfWork.Messages
                              .GetAllAsync(m => m.SenderId == userId || m.ReceiverId == userId,
                              includeProperties: "Sender,Receiver",
                              orderBy: m => m.OrderByDescending(m => m.SentAt));


            var senders = chats.Select(m =>
            {
                var sender = new SenderDto();

                // get reciever details
                if (m.SenderId == userId)
                {
                    sender.SenderId = m.ReceiverId;
                    sender.SenderName = m.Receiver.Name;
                    sender.SenderImage = m.Receiver.Image;

                }
                //get sender details
                else if (m.ReceiverId == userId)
                {
                    sender.SenderId = m.SenderId;
                    sender.SenderName = m.Sender.Name;
                    sender.SenderImage = m.Sender.Image;
                }
                return sender;

            })
                .DistinctBy(c => c.SenderId)
                .ToList();

            return senders;
        }

        private async Task<bool> IsExistAsync(int senderId, int receiverId)
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            bool isSenderExist = users.Any(u => u.Id == senderId);
            bool isReceiverExist = users.Any(u => u.Id == receiverId);

            return isSenderExist && isReceiverExist;
        }

        private async Task<bool> IsMessagedBeforeInNotification(string SenderName,int recieverId)
        {
            var notifications =  await _unitOfWork.Notifications.GetAllAsync(n => n.UserId == recieverId && n.NotificationType == "Message");
            return notifications.Any(n => n.Description.Contains(SenderName));
        }
    }
}
