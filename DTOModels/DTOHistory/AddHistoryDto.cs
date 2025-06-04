namespace RentMateAPI.DTOModels.DTOHistory
{
    public class AddHistoryDto
    {
        public int UserId { get; set; }
        public string Description { get; set; } = null!;
        public string HistoryType { get; set; } = null!;
    }
}
