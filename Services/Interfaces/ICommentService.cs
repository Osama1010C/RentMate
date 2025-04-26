using RentMateAPI.DTOModels.DTOComment;

namespace RentMateAPI.Services.Interfaces
{
    public interface ICommentService
    {
        Task<List<UserCommentDto>> GetAllPropertyCommentsAsync(int propertyId);

        Task AddCommentAsync(int userId, int propertyId, string comment);
    }
}
