namespace RentMateAPI.DTOModels.DTOComment
{
    public class NewCommentDto
    {
        public int UserId { get; set; }

        public int PropertyId { get; set; }

        public string Content { get; set; } = null!;
    }
}
