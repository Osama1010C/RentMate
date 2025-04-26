namespace RentMateAPI.DTOModels.DTOComment
{
    public class UserCommentDto
    {
        public int UserId { get; set; }
        public string Name { get; set; } = null!;
        public byte[]? Image { get; set; }
        public string Role { get; set; } = null!;
        public string CommentContent { get; set; } = null!;
        public DateTime? CreateAt { get; set; }
    }
}
