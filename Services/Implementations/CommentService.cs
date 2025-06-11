using RentMateAPI.Data.Models;
using RentMateAPI.DTOModels.DTOComment;
using RentMateAPI.Services.Interfaces;
using RentMateAPI.UOF.Interface;
using RentMateAPI.Validations.Interfaces;

namespace RentMateAPI.Services.Implementations
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IModelValidator<User> _userValidator;
        private readonly IModelValidator<Property> _propertyValidator;
        public CommentService(IUnitOfWork unitOfWork, IModelValidator<User> userValidator, IModelValidator<Property> propertyValidator)
        {
            this._unitOfWork = unitOfWork;
            this._userValidator = userValidator;
            this._propertyValidator = propertyValidator;
        }

        public async Task AddCommentAsync(int userId, int propertyId, string comment)
        {
            await _userValidator.IsModelExist(userId);
            await _propertyValidator.IsModelExist(propertyId);

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
            var property = await _propertyValidator.IsModelExistReturn(propertyId);

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
    }
}
