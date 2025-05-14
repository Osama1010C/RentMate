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

        [HttpGet("MyChats/{userId}")]
        public async Task<IActionResult> GetAllTenantChats(int userId)
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
