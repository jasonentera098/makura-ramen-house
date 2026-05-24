using System.Data.OleDb;
using System.Text;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk
{
    public class TicketRepository : RepositoryBase, ITicketRepository
    {
        public TicketRepository(string connectionString) : base(connectionString) { }

        private static Ticket MapTicket(OleDbDataReader reader) => new Ticket
        {
            TicketID      = reader.GetInt32(reader.GetOrdinal("TicketID")),
            Title         = reader["Title"]?.ToString() ?? string.Empty,
            Description   = reader["Description"]?.ToString() ?? string.Empty,
            Category      = reader["Category"]?.ToString() ?? string.Empty,
            Priority      = reader["Priority"]?.ToString() ?? string.Empty,
            Status        = reader["Status"]?.ToString() ?? string.Empty,
            DateSubmitted = reader["DateSubmitted"] == DBNull.Value
                                ? DateTime.MinValue
                                : Convert.ToDateTime(reader["DateSubmitted"]),
            DateResolved  = reader["DateResolved"] == DBNull.Value
                                ? (DateTime?)null
                                : Convert.ToDateTime(reader["DateResolved"]),
            SubmittedBy   = Convert.ToInt32(reader["SubmittedBy"]),
            AssignedTo    = reader["AssignedTo"] == DBNull.Value
                                ? (int?)null
                                : Convert.ToInt32(reader["AssignedTo"]),
            DepartmentID  = Convert.ToInt32(reader["DepartmentID"])
        };

        private const string SelectColumns =
            "SELECT TicketID, Title, Description, Category, Priority, Status, " +
            "DateSubmitted, DateResolved, SubmittedBy, AssignedTo, DepartmentID " +
            "FROM Tickets";

        public async Task<int> InsertAsync(Ticket ticket)
        {
            var dateResolvedParam = new OleDbParameter("@DateResolved", OleDbType.Date)
                { Value = (object?)ticket.DateResolved ?? DBNull.Value };
            var assignedToParam = new OleDbParameter("@AssignedTo", OleDbType.Integer)
                { Value = (object?)ticket.AssignedTo ?? DBNull.Value };

            await ExecuteNonQueryAsync(
                "INSERT INTO Tickets (Title, Description, Category, Priority, Status, " +
                "DateSubmitted, DateResolved, SubmittedBy, AssignedTo, DepartmentID) " +
                "VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)",
                [
                    new OleDbParameter("@Title",         OleDbType.VarWChar) { Value = ticket.Title },
                    new OleDbParameter("@Description",   OleDbType.LongVarWChar) { Value = ticket.Description },
                    new OleDbParameter("@Category",      OleDbType.VarWChar) { Value = ticket.Category },
                    new OleDbParameter("@Priority",      OleDbType.VarWChar) { Value = ticket.Priority },
                    new OleDbParameter("@Status",        OleDbType.VarWChar) { Value = ticket.Status },
                    new OleDbParameter("@DateSubmitted", OleDbType.Date) { Value = ticket.DateSubmitted },
                    dateResolvedParam,
                    new OleDbParameter("@SubmittedBy",   OleDbType.Integer) { Value = ticket.SubmittedBy },
                    assignedToParam,
                    new OleDbParameter("@DepartmentID",  OleDbType.Integer) { Value = ticket.DepartmentID }
                ]);

            return await ExecuteScalarAsync<int>("SELECT @@IDENTITY");
        }

        public async Task<bool> UpdateAsync(Ticket ticket)
        {
            var dateResolvedParam = new OleDbParameter("@DateResolved", OleDbType.Date)
                { Value = (object?)ticket.DateResolved ?? DBNull.Value };
            var assignedToParam = new OleDbParameter("@AssignedTo", OleDbType.Integer)
                { Value = (object?)ticket.AssignedTo ?? DBNull.Value };

            int rows = await ExecuteNonQueryAsync(
                "UPDATE Tickets SET Title = ?, Description = ?, Category = ?, Priority = ?, " +
                "Status = ?, DateSubmitted = ?, DateResolved = ?, SubmittedBy = ?, " +
                "AssignedTo = ?, DepartmentID = ? WHERE TicketID = ?",
                [
                    new OleDbParameter("@Title",         OleDbType.VarWChar) { Value = ticket.Title },
                    new OleDbParameter("@Description",   OleDbType.LongVarWChar) { Value = ticket.Description },
                    new OleDbParameter("@Category",      OleDbType.VarWChar) { Value = ticket.Category },
                    new OleDbParameter("@Priority",      OleDbType.VarWChar) { Value = ticket.Priority },
                    new OleDbParameter("@Status",        OleDbType.VarWChar) { Value = ticket.Status },
                    new OleDbParameter("@DateSubmitted", OleDbType.Date) { Value = ticket.DateSubmitted },
                    dateResolvedParam,
                    new OleDbParameter("@SubmittedBy",   OleDbType.Integer) { Value = ticket.SubmittedBy },
                    assignedToParam,
                    new OleDbParameter("@DepartmentID",  OleDbType.Integer) { Value = ticket.DepartmentID },
                    new OleDbParameter("@TicketID",      OleDbType.Integer) { Value = ticket.TicketID }
                ]);
            return rows > 0;
        }

        public Task<Ticket?> GetByIdAsync(int ticketId) =>
            ExecuteReaderAsync(
                SelectColumns + " WHERE TicketID = ?",
                MapTicket,
                [new OleDbParameter("@TicketID", ticketId)])
            .ContinueWith(t => t.Result.FirstOrDefault());

        public Task<List<Ticket>> GetBySubmitterAsync(int userId) =>
            ExecuteReaderAsync(
                SelectColumns + " WHERE SubmittedBy = ? ORDER BY DateSubmitted DESC",
                MapTicket,
                [new OleDbParameter("@SubmittedBy", userId)]);

        public Task<List<Ticket>> GetByAssigneeAsync(int userId) =>
            ExecuteReaderAsync(
                SelectColumns + " WHERE AssignedTo = ? ORDER BY DateSubmitted DESC",
                MapTicket,
                [new OleDbParameter("@AssignedTo", userId)]);

        public Task<List<Ticket>> GetByStatusAsync(string status) =>
            ExecuteReaderAsync(
                SelectColumns + " WHERE Status = ? ORDER BY DateSubmitted DESC",
                MapTicket,
                [new OleDbParameter("@Status", status)]);

        public Task<List<Ticket>> GetAllAsync() =>
            ExecuteReaderAsync(
                SelectColumns + " ORDER BY DateSubmitted DESC",
                MapTicket);

        public Task<List<Ticket>> SearchAsync(string searchTerm, Dictionary<string, object> filters)
        {
            var sql = new StringBuilder(SelectColumns + " WHERE (");
            var parameters = new List<OleDbParameter>();

            // Search term: partial match on Title/Description, or exact TicketID if numeric
            if (!string.IsNullOrWhiteSpace(searchTerm) && int.TryParse(searchTerm.Trim(), out int ticketId))
            {
                sql.Append("TicketID = ?");
                parameters.Add(new OleDbParameter("@TicketID", ticketId));
            }
            else if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string like = $"%{searchTerm.Trim()}%";
                sql.Append("Title LIKE ? OR Description LIKE ?");
                parameters.Add(new OleDbParameter("@Title", like));
                parameters.Add(new OleDbParameter("@Description", like));
            }
            else
            {
                sql.Append("1=1");
            }

            sql.Append(")");

            // Apply filters (AND logic)
            if (filters != null)
            {
                if (filters.TryGetValue("Status", out var status))
                {
                    sql.Append(" AND Status = ?");
                    parameters.Add(new OleDbParameter("@Status", status.ToString()));
                }

                if (filters.TryGetValue("Priority", out var priority))
                {
                    sql.Append(" AND Priority = ?");
                    parameters.Add(new OleDbParameter("@Priority", priority.ToString()));
                }

                if (filters.TryGetValue("Category", out var category))
                {
                    sql.Append(" AND Category = ?");
                    parameters.Add(new OleDbParameter("@Category", category.ToString()));
                }

                if (filters.TryGetValue("DateFrom", out var dateFrom) && dateFrom is DateTime dtFrom)
                {
                    sql.Append(" AND DateSubmitted >= ?");
                    parameters.Add(new OleDbParameter("@DateFrom", OleDbType.Date) { Value = dtFrom });
                }

                if (filters.TryGetValue("DateTo", out var dateTo) && dateTo is DateTime dtTo)
                {
                    sql.Append(" AND DateSubmitted <= ?");
                    parameters.Add(new OleDbParameter("@DateTo", OleDbType.Date) { Value = dtTo });
                }

                if (filters.TryGetValue("SubmittedBy", out var submittedBy))
                {
                    sql.Append(" AND SubmittedBy = ?");
                    parameters.Add(new OleDbParameter("@SubmittedBy", Convert.ToInt32(submittedBy)));
                }

                if (filters.TryGetValue("AssignedTo", out var assignedTo))
                {
                    sql.Append(" AND AssignedTo = ?");
                    parameters.Add(new OleDbParameter("@AssignedTo", Convert.ToInt32(assignedTo)));
                }
            }

            sql.Append(" ORDER BY DateSubmitted DESC");

            return ExecuteReaderAsync(sql.ToString(), MapTicket, [.. parameters]);
        }
    }
}
