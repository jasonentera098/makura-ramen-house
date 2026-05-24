using IT_Helpdesk.Models;

namespace IT_Helpdesk.Repositories
{
    public interface ITicketRepository
    {
        Task<int> InsertAsync(Ticket ticket);
        Task<bool> UpdateAsync(Ticket ticket);
        Task<Ticket?> GetByIdAsync(int ticketId);
        Task<List<Ticket>> GetBySubmitterAsync(int userId);
        Task<List<Ticket>> GetByAssigneeAsync(int userId);
        Task<List<Ticket>> GetByStatusAsync(string status);
        Task<List<Ticket>> SearchAsync(string searchTerm, Dictionary<string, object> filters);
        Task<List<Ticket>> GetAllAsync();
    }
}
