using IT_Helpdesk.Models;

namespace IT_Helpdesk.Repositories
{
    public interface ITicketUpdateRepository
    {
        Task<int> InsertAsync(TicketUpdate ticketUpdate);
        Task<List<TicketUpdate>> GetByTicketIdAsync(int ticketId);
    }
}
