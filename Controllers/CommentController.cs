using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RentMateAPI.DTOModels.DTOComment;
using RentMateAPI.Services.Interfaces;

namespace RentMateAPI.Controllers
{
    [Route("RentMate/[controller]")]
    [ApiController]
    [Authorize(Roles = "tenant,landlord")]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        public CommentController(ICommentService commentService)
        {
            this._commentService = commentService;
        }



        [HttpGet("{propertyId}")]
        public async Task<IActionResult> GetAllPropertyComments(int propertyId)
        {
            try
            {
                var comments = await _commentService.GetAllPropertyCommentsAsync(propertyId);
                return Ok(comments);
            }
            catch (Exception ex)
            {
                return NotFound(ex.Message);
            }
        }

        

        [HttpPost]
        public async Task<IActionResult> AddComment(NewCommentDto newComment)
        {
            try
            {
                await _commentService.AddCommentAsync(newComment.UserId, newComment.PropertyId, newComment.Content);
                return Ok();
            }
            catch(Exception ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
