using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly ITicketUpdateRepository _ticketUpdateRepository;
        private readonly IUserRepository _userRepository;

        // Valid status transitions: Pending → In Progress → Resolved → Closed
        private static readonly Dictionary<string, string> ValidTransitions = new()
        {
            { "Pending",     "In Progress" },
            { "In Progress", "Resolved"    },
            { "Resolved",    "Closed"      }
        };

        public TicketService(
            ITicketRepository ticketRepository,
            ITicketUpdateRepository ticketUpdateRepository,
            IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _ticketUpdateRepository = ticketUpdateRepository;
            _userRepository = userRepository;
        }

        public async Task<int> CreateTicketAsync(Ticket ticket, int submittedByUserId, int departmentId)
        {
            // Auto-set system-generated fields
            ticket.Status = "Pending";
            ticket.DateSubmitted = DateTime.Now;
            ticket.SubmittedBy = submittedByUserId;
            ticket.DepartmentID = departmentId;
            ticket.DateResolved = null;
            ticket.AssignedTo = null;

            return await _ticketRepository.InsertAsync(ticket);
        }

        public Task<bool> UpdateTicketAsync(Ticket ticket) =>
            _ticketRepository.UpdateAsync(ticket);

        public async Task<bool> AssignTicketAsync(int ticketId, int technicianId, int assignedBy)
        {
            // Validate that the target user exists and is a Technician
            var technician = await _userRepository.GetByIdAsync(technicianId);
            if (technician == null || technician.Role != "Technician")
                return false;

            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
                return false;

            ticket.AssignedTo = technicianId;

            // Move to In Progress if still Pending
            string statusChange = string.Empty;
            if (ticket.Status == "Pending")
            {
                ticket.Status = "In Progress";
                statusChange = "Pending → In Progress";
            }

            bool updated = await _ticketRepository.UpdateAsync(ticket);
            if (!updated)
                return false;

            // Record the assignment as a TicketUpdate
            await _ticketUpdateRepository.InsertAsync(new TicketUpdate
            {
                TicketID     = ticketId,
                UpdatedBy    = assignedBy,
                Remarks      = $"Ticket assigned to {technician.FullName}.",
                UpdateDate   = DateTime.Now,
                StatusChange = statusChange
            });

            return true;
        }

        public async Task<bool> UpdateStatusAsync(int ticketId, string newStatus, int updatedBy)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
                return false;

            // Enforce valid lifecycle transition
            if (!ValidTransitions.TryGetValue(ticket.Status, out var allowedNext) || allowedNext != newStatus)
                return false;

            string statusChange = $"{ticket.Status} → {newStatus}";
            ticket.Status = newStatus;

            if (newStatus == "Resolved")
                ticket.DateResolved = DateTime.Now;

            bool updated = await _ticketRepository.UpdateAsync(ticket);
            if (!updated)
                return false;

            await _ticketUpdateRepository.InsertAsync(new TicketUpdate
            {
                TicketID     = ticketId,
                UpdatedBy    = updatedBy,
                Remarks      = $"Status changed to {newStatus}.",
                UpdateDate   = DateTime.Now,
                StatusChange = statusChange
            });

            return true;
        }

        public async Task<bool> AddRemarkAsync(int ticketId, string remarks, int updatedBy)
        {
            var ticket = await _ticketRepository.GetByIdAsync(ticketId);
            if (ticket == null)
                return false;

            await _ticketUpdateRepository.InsertAsync(new TicketUpdate
            {
                TicketID     = ticketId,
                UpdatedBy    = updatedBy,
                Remarks      = remarks,
                UpdateDate   = DateTime.Now,
                StatusChange = string.Empty
            });

            return true;
        }

        public Task<List<Ticket>> GetTicketsByUserAsync(int userId, string role) =>
            role switch
            {
                "Employee"      => _ticketRepository.GetBySubmitterAsync(userId),
                "Technician"    => _ticketRepository.GetByAssigneeAsync(userId),
                "Administrator" => _ticketRepository.GetAllAsync(),
                _               => _ticketRepository.GetAllAsync()
            };

        public Task<List<Ticket>> SearchTicketsAsync(string searchTerm, Dictionary<string, object> filters) =>
            _ticketRepository.SearchAsync(searchTerm, filters);

        public Task<Ticket?> GetTicketByIdAsync(int ticketId) =>
            _ticketRepository.GetByIdAsync(ticketId);
    }
}
