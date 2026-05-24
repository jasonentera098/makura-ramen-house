namespace IT_Helpdesk.Models
{
    public class Ticket
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // Hardware, Software, Network, Access, Other
        public string Priority { get; set; } = string.Empty; // High, Medium, Low
        public string Status { get; set; } = string.Empty; // Pending, In Progress, Resolved, Closed
        public DateTime DateSubmitted { get; set; }
        public DateTime? DateResolved { get; set; }
        public int SubmittedBy { get; set; }
        public int? AssignedTo { get; set; }
        public int DepartmentID { get; set; }
    }
}
