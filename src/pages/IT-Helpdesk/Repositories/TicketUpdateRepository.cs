using System.Data.OleDb;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk
{
    public class TicketUpdateRepository : RepositoryBase, ITicketUpdateRepository
    {
        public TicketUpdateRepository(string connectionString) : base(connectionString) { }

        private static TicketUpdate MapTicketUpdate(OleDbDataReader reader) => new TicketUpdate
        {
            UpdateID     = reader.GetInt32(reader.GetOrdinal("UpdateID")),
            TicketID     = reader.GetInt32(reader.GetOrdinal("TicketID")),
            UpdatedBy    = reader.GetInt32(reader.GetOrdinal("UpdatedBy")),
            Remarks      = reader["Remarks"]?.ToString() ?? string.Empty,
            UpdateDate   = reader["UpdateDate"] == DBNull.Value
                               ? DateTime.MinValue
                               : Convert.ToDateTime(reader["UpdateDate"]),
            StatusChange = reader["StatusChange"] == DBNull.Value
                               ? string.Empty
                               : reader["StatusChange"]?.ToString() ?? string.Empty
        };

        public async Task<int> InsertAsync(TicketUpdate ticketUpdate)
        {
            await ExecuteNonQueryAsync(
                "INSERT INTO TicketUpdates (TicketID, UpdatedBy, Remarks, UpdateDate, StatusChange) " +
                "VALUES (?, ?, ?, ?, ?)",
                [
                    new OleDbParameter("@TicketID",     OleDbType.Integer) { Value = ticketUpdate.TicketID },
                    new OleDbParameter("@UpdatedBy",    OleDbType.Integer) { Value = ticketUpdate.UpdatedBy },
                    new OleDbParameter("@Remarks",      OleDbType.LongVarWChar) { Value = ticketUpdate.Remarks },
                    new OleDbParameter("@UpdateDate",   OleDbType.Date) { Value = ticketUpdate.UpdateDate },
                    new OleDbParameter("@StatusChange", OleDbType.VarWChar)
                    {
                        Value = string.IsNullOrEmpty(ticketUpdate.StatusChange)
                            ? DBNull.Value
                            : (object)ticketUpdate.StatusChange
                    }
                ]);

            return await ExecuteScalarAsync<int>("SELECT @@IDENTITY");
        }

        public Task<List<TicketUpdate>> GetByTicketIdAsync(int ticketId) =>
            ExecuteReaderAsync(
                "SELECT UpdateID, TicketID, UpdatedBy, Remarks, UpdateDate, StatusChange " +
                "FROM TicketUpdates WHERE TicketID = ? ORDER BY UpdateDate ASC",
                MapTicketUpdate,
                [new OleDbParameter("@TicketID", ticketId)]);
    }
}
