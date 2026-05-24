namespace IT_Helpdesk.Models
{
    public class User
    {
        public int UserID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // "Employee", "Technician", "Administrator"
        public int DepartmentID { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string AccountStatus { get; set; } = "Active"; // "Active" or "Inactive"
        public DateTime CreatedAt { get; set; }
    }
}
