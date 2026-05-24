using IT_Helpdesk.Models;

namespace IT_Helpdesk.Services
{
    /// <summary>
    /// Summary statistics returned by GetTicketSummary.
    /// </summary>
    public class TicketSummary
    {
        public string GroupKey { get; set; } = string.Empty; // Category or Status value
        public int Total { get; set; }
        public int Resolved { get; set; }
        public int Pending { get; set; }
    }

    /// <summary>
    /// Per-technician performance stats returned by GetTechnicianPerformance.
    /// </summary>
    public class TechnicianPerformance
    {
        public int TechnicianID { get; set; }
        public string FullName { get; set; } = string.Empty;
        public int Assigned { get; set; }
        public int Resolved { get; set; }
        public int Pending { get; set; }
        public double AverageResolutionTimeHours { get; set; }
    }

    public interface IReportService
    {
        /// <summary>
        /// Returns ticket counts grouped by category (when category is null/empty) or
        /// by status (when category is specified), filtered by date range and optional status.
        /// </summary>
        Task<List<TicketSummary>> GetTicketSummaryAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? category,
            string? status);

        /// <summary>
        /// Returns a filtered list of tickets scoped by the caller's role:
        /// Employee → own submitted tickets, Technician → assigned tickets, Admin → all tickets.
        /// </summary>
        Task<List<Ticket>> GetTicketsByFiltersAsync(
            DateTime? fromDate,
            DateTime? toDate,
            string? category,
            string? status,
            string? priority,
            int? departmentId,
            int? userId,
            string userRole);

        /// <summary>
        /// Returns per-technician stats (assigned, resolved, pending counts) for the given date range.
        /// </summary>
        Task<List<TechnicianPerformance>> GetTechnicianPerformanceAsync(
            DateTime? fromDate,
            DateTime? toDate);
    }
}
