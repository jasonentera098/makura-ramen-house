namespace IT_Helpdesk.Models
{
    public class LoginLog
    {
        public int LogID { get; set; }
        public int UserID { get; set; }
        public DateTime LoginTime { get; set; }
        public DateTime? LogoutTime { get; set; }
    }
}
