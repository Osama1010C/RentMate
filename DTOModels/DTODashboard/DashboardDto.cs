namespace RentMateAPI.DTOModels.DTODashboard
{
    public class DashboardDto
    {
        public int NumberOfUsers { get; set; }
        public int NumberOfAdmins { get; set; }
        public int NumberOfTenants { get; set; }
        public int NumberOfLandlords { get; set; }
        public int NumberOfPendingLandlordRegistrations { get; set; }
        public int NumberOfProperties { get; set; }
        public int NumberOfPendingProperties { get; set; }
    }
}
