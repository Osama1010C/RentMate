using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    [Authorize(Roles = "tenant,landlord")]

    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;

        public MessageController(IMessageService messageService)
        {
            this._messageService = messageService;
        }

        /// <summary>Return all chats for a specific user</summary>
        [HttpGet("MyChats/{userId}")]
        public async Task<IActionResult> GetAllUserChats(int userId)
        {
            try
            {
                return Ok(await _messageService.GetMyChatsAsync(userId));
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>Return content of the chat between two users</summary>
        [HttpGet("Chatting")]
        public async Task<IActionResult> GetChatContent(int userId, int recieverId)
        {
            try
            {
                return Ok(await _messageService.GetChatContentAsync(userId, recieverId));
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }


        /// <summary>Send a message from one user to another</summary>
        [HttpPost("SendMessage")]
        public async Task<IActionResult> AddMessage(int senderId, int recieverId, string message)
        {
            try
            {
                await _messageService.AddMessageAsync(senderId, recieverId, message);
                return Ok();
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
