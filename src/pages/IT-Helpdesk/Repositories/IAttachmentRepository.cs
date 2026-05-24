using IT_Helpdesk.Models;

namespace IT_Helpdesk.Repositories
{
    public interface IAttachmentRepository
    {
        Task<int> InsertAsync(Attachment attachment);
        Task<List<Attachment>> GetByTicketIdAsync(int ticketId);
        Task<bool> DeleteAsync(int attachmentId);
    }
}
