namespace IT_Helpdesk.Models
{
    public class TicketUpdate
    {
        public int UpdateID { get; set; }
        public int TicketID { get; set; }
        public int UpdatedBy { get; set; }
        public string Remarks { get; set; } = string.Empty;
        public DateTime UpdateDate { get; set; }
        public string StatusChange { get; set; } = string.Empty; // Optional: tracks status transitions
    }
}
