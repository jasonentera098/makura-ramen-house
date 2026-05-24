namespace IT_Helpdesk.Models
{
    public class TechnicianWorkloadInfo
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Status { get; set; } = "Available"; // Available, Busy, Overloaded, On Leave
        public int AssignedTicketsCount { get; set; }
        public int WorkloadPercentage { get; set; }
        public double WorkloadBarWidth { get; set; }
        public string AccountStatus { get; set; } = "Active";
    }
}
