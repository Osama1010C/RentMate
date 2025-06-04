namespace RentMateAPI.DTOModels.DTOHistory
{
    public class GetHistoryDto
    {
        public string Description { get; set; } = null!;
        public string HistoryType { get; set; } = null!;
        public DateTime? ActionDate { get; set; }

    }
}
