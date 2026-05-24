namespace IT_Helpdesk.Models
{
    public class Attachment
    {
        public int AttachmentID { get; set; }
        public int TicketID { get; set; }
        public string FilePath { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public int UploadedBy { get; set; }
        public DateTime UploadedAt { get; set; }
    }
}
