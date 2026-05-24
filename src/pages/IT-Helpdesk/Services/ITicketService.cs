using IT_Helpdesk.Models;

namespace IT_Helpdesk.Services
{
    public interface ITicketService
    {
        Task<int> CreateTicketAsync(Ticket ticket, int submittedByUserId, int departmentId);
        Task<bool> UpdateTicketAsync(Ticket ticket);
        Task<bool> AssignTicketAsync(int ticketId, int technicianId, int assignedBy);
        Task<bool> UpdateStatusAsync(int ticketId, string newStatus, int updatedBy);
        Task<bool> AddRemarkAsync(int ticketId, string remarks, int updatedBy);
        Task<List<Ticket>> GetTicketsByUserAsync(int userId, string role);
        Task<List<Ticket>> SearchTicketsAsync(string searchTerm, Dictionary<string, object> filters);
        Task<Ticket?> GetTicketByIdAsync(int ticketId);
    }
}
