using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.SignalR;
using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOMessage;
using RentMateAPI.DTOModels.DTONotification;
using RentMateAPI.Helpers;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using RentMateAPI.Validations.Interfaces;

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
        private readonly IModelValidator<User> _userValidator;

        public MessageService(IUnitOfWork unitOfWork, IHubContext<ChatHub> hubContext,
            IDataProtectionProvider dataProtectionProvider, IConfiguration configuration,
            INotificationService notificationService, IModelValidator<User> userValidator)
        {
            _unitOfWork = unitOfWork;
            _hubContext = hubContext;
            _dataProtector = dataProtectionProvider.CreateProtector(configuration["SecretKey"]!);
            _notificationService = notificationService;
            _userValidator = userValidator;
        }

        public async Task AddMessageAsync(int senderId, int recieverId, string message)
        {
            await _userValidator.IsModelExist(senderId);
            await _userValidator.IsModelExist(recieverId);

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

            var IsMessagedBefore = await NotificationHelper.IsMessagedBeforeInNotification(_unitOfWork, messageDto.SenderName, recieverId);
            if (!IsMessagedBefore)
            {
                var notificationDto = new AddNotificationDto
                {
                    Description = $"{messageDto.SenderName} sent you a message",
                    NotificationType = "Message",
                    NotificationTypeId = msg.SenderId
                };
                await _notificationService.AddNotificationAsync(recieverId, notificationDto);
                await _unitOfWork.CompleteAsync();
            }
        }

        public async Task<List<MessageDto>> GetChatContentAsync(int userId, int recieverId)
        {
            await _userValidator.IsModelExist(userId);
            await _userValidator.IsModelExist(recieverId);


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

            foreach (var message in chat)
                message.Seen = 1;

            await _unitOfWork.CompleteAsync();


            return messages;
        }

        public async Task<List<SenderDto>> GetMyChatsAsync(int userId)
        {
            await _userValidator.IsModelExist(userId);


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

            var detailedSender = new List<SenderDto>();

            foreach (var sender in senders)
            {
                var chat = await _unitOfWork.Messages
                              .GetAllAsync(m => (m.SenderId == userId || m.ReceiverId == userId)
                              && (m.SenderId == sender.SenderId || m.ReceiverId == sender.SenderId));
                if (chat.Any(c => c.Seen == 0))
                {
                    sender.IsAnyUnseenMessages = true;
                    sender.NumberOfUnseenMessages = chat.Count(c => c.Seen == 0);
                }
                //sender.LastMessage = ShapeLargeMessgae(await GetLastMessage(sender.SenderId, userId));
                var lastMessage = _dataProtector.Unprotect(await MessageHelper.GetLastMessage(_unitOfWork, sender.SenderId, userId));
                sender.LastMessage = MessageHelper.ShapeLargeMessgae(lastMessage);

                detailedSender.Add(sender);
            }

            return detailedSender;
        }

        
    }
}
