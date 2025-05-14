using RentMateAPI.DTOModels.DTOComment;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;

namespace RentMateAPI.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public CommentService(IUnitOfWork unitOfWork)
        {
            this._unitOfWork = unitOfWork;
        }

        public async Task AddCommentAsync(int userId, int propertyId, string comment)
        {
            if (!await IsExistAsync(userId, propertyId))
                throw new Exception("this user or property id not found");
            
            await _unitOfWork.Comments.AddAsync(new()
            {
                UserId = userId,
                PropertyId = propertyId,
                Content = comment
            });

            await _unitOfWork.CompleteAsync();
        }

        public async Task<List<UserCommentDto>> GetAllPropertyCommentsAsync(int propertyId)
        {
            var property = await _unitOfWork.Properties
                            .GetByIdAsync(propertyId);

            if (property is null) throw new Exception("this property id not found");

            var comments = await _unitOfWork.Comments
                           .GetAllAsync(c => c.PropertyId == propertyId, includeProperties: "User");

            var commentDtos = comments.Select(c => new UserCommentDto
            {
                UserId = c.User.Id,
                Name = c.User.Name,
                Image = c.User.Image,
                Role = c.User.Role,
                CommentContent = c.Content,
                CreateAt = c.CreateAt
            }).ToList();

            return commentDtos;
        }


        private async Task<bool> IsExistAsync(int userId, int propertyId)
        {
            var isUserExist = await _unitOfWork.Users.IsExistAsync(userId);
            
            var isPropertyExist = await _unitOfWork.Properties.IsExistAsync(propertyId);

            return isUserExist && isPropertyExist;

        }
    }
}
