using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk.Services
{
    public class ReportService : IReportService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IUserRepository _userRepository;

        public ReportService(ITicketRepository ticketRepository, IUserRepository userRepository)
        {
            _ticketRepository = ticketRepository;
            _userRepository = userRepository;
        }

        /// <inheritdoc/>
        public async Task<List<TicketSummary>> GetTicketSummaryAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? category,
            string? status)
        {
            // Fetch all tickets then apply in-memory filtering and grouping.
            // MS Access does not support complex GROUP BY via the existing repository interface,
            // so we reuse the Search method with appropriate filters.
            var filters = BuildFilters(fromDate, toDate, category, status, null, null, null);
            var tickets = await _ticketRepository.SearchAsync(string.Empty, filters);

            // Group by category when no specific category is requested; otherwise group by status.
            if (string.IsNullOrWhiteSpace(category))
            {
                return tickets
                    .GroupBy(t => t.Category)
                    .Select(g => new TicketSummary
                    {
                        GroupKey = g.Key,
                        Total    = g.Count(),
                        Resolved = g.Count(t => t.Status == "Resolved" || t.Status == "Closed"),
                        Pending  = g.Count(t => t.Status == "Pending" || t.Status == "In Progress")
                    })
                    .OrderBy(s => s.GroupKey)
                    .ToList();
            }
            else
            {
                return tickets
                    .GroupBy(t => t.Status)
                    .Select(g => new TicketSummary
                    {
                        GroupKey = g.Key,
                        Total    = g.Count(),
                        Resolved = g.Count(t => t.Status == "Resolved" || t.Status == "Closed"),
                        Pending  = g.Count(t => t.Status == "Pending" || t.Status == "In Progress")
                    })
                    .OrderBy(s => s.GroupKey)
                    .ToList();
            }
        }

        /// <inheritdoc/>
        public async Task<List<Ticket>> GetTicketsByFiltersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? category,
            string? status,
            string? priority,
            int? departmentId,
            int? userId,
            string userRole)
        {
            var filters = BuildFilters(fromDate, toDate, category, status, priority, null, null);

            // Apply role-based scoping
            switch (userRole)
            {
                case "Employee":
                    if (userId.HasValue)
                        filters["SubmittedBy"] = userId.Value;
                    break;

                case "Technician":
                    if (userId.HasValue)
                        filters["AssignedTo"] = userId.Value;
                    break;

                // Administrator sees all tickets — no additional scope filter
            }

            var tickets = await _ticketRepository.SearchAsync(string.Empty, filters);

            // DepartmentID is not supported by SearchAsync, so filter in-memory
            if (departmentId.HasValue)
                tickets = tickets.Where(t => t.DepartmentID == departmentId.Value).ToList();

            return tickets;
        }

        /// <inheritdoc/>
        public async Task<List<TechnicianPerformance>> GetTechnicianPerformanceAsync(
            DateTime? fromDate,
            DateTime? toDate)
        {
            // Load all technicians and all tickets in the date range, then join in memory.
            var technicians = await _userRepository.GetByRoleAsync("Technician");

            var filters = BuildFilters(fromDate, toDate, null, null, null, null, null);
            var tickets = await _ticketRepository.SearchAsync(string.Empty, filters);

            // Build a lookup: technicianId → list of assigned tickets
            var ticketsByTech = tickets
                .Where(t => t.AssignedTo.HasValue)
                .GroupBy(t => t.AssignedTo!.Value)
                .ToDictionary(g => g.Key, g => g.ToList());

            return technicians
                .Select(tech =>
                {
                    ticketsByTech.TryGetValue(tech.UserID, out var assigned);
                    assigned ??= new List<Ticket>();

                    // Calculate average resolution time for resolved tickets
                    var resolvedTickets = assigned.Where(t => 
                        (t.Status == "Resolved" || t.Status == "Closed") && 
                        t.DateResolved.HasValue).ToList();

                    double averageResolutionTimeHours = 0;
                    if (resolvedTickets.Count > 0)
                    {
                        var totalHours = resolvedTickets
                            .Sum(t => (t.DateResolved!.Value - t.DateSubmitted).TotalHours);
                        averageResolutionTimeHours = totalHours / resolvedTickets.Count;
                    }

                    return new TechnicianPerformance
                    {
                        TechnicianID = tech.UserID,
                        FullName     = tech.FullName,
                        Assigned     = assigned.Count,
                        Resolved     = assigned.Count(t => t.Status == "Resolved" || t.Status == "Closed"),
                        Pending      = assigned.Count(t => t.Status == "Pending" || t.Status == "In Progress"),
                        AverageResolutionTimeHours = averageResolutionTimeHours
                    };
                })
                .OrderBy(p => p.FullName)
                .ToList();
        }

        // ── Helpers ──────────────────────────────────────────────────────────────

        private static Dictionary<string, object> BuildFilters(
            DateTime? fromDate,
            DateTime? toDate,
            string? category,
            string? status,
            string? priority,
            int? departmentId,
            int? submittedBy)
        {
            var filters = new Dictionary<string, object>();

            if (fromDate.HasValue)
                filters["DateFrom"] = fromDate.Value;

            if (toDate.HasValue)
                filters["DateTo"] = toDate.Value;

            if (!string.IsNullOrWhiteSpace(category))
                filters["Category"] = category;

            if (!string.IsNullOrWhiteSpace(status))
                filters["Status"] = status;

            if (!string.IsNullOrWhiteSpace(priority))
                filters["Priority"] = priority;

            if (submittedBy.HasValue)
                filters["SubmittedBy"] = submittedBy.Value;

            return filters;
        }
    }
}
