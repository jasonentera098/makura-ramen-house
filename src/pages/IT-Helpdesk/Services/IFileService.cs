using IT_Helpdesk.Models;

namespace IT_Helpdesk.Services
{
    public interface IFileService
    {
        Task<string> SaveAttachmentAsync(string sourceFilePath, int ticketId);
        Task<bool> DeleteAttachmentAsync(string filePath);
        Task<bool> OpenAttachmentAsync(string filePath);
        Task<List<Attachment>> GetAttachmentsByTicketAsync(int ticketId);
    }
}
