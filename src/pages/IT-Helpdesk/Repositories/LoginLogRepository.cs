using System.Data.OleDb;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk
{
    public class LoginLogRepository : RepositoryBase, ILoginLogRepository
    {
        public LoginLogRepository(string connectionString) : base(connectionString) { }

        private static LoginLog MapLoginLog(OleDbDataReader reader) => new LoginLog
        {
            LogID       = reader.GetInt32(reader.GetOrdinal("LogID")),
            UserID      = reader.GetInt32(reader.GetOrdinal("UserID")),
            LoginTime   = Convert.ToDateTime(reader["LoginTime"]),
            LogoutTime  = reader["LogoutTime"] == DBNull.Value
                              ? (DateTime?)null
                              : Convert.ToDateTime(reader["LogoutTime"])
        };

        public async Task<int> InsertLoginAsync(int userId)
        {
            await ExecuteNonQueryAsync(
                "INSERT INTO LoginLogs (UserID, LoginTime) VALUES (?, ?)",
                [
                    new OleDbParameter("@UserID",    OleDbType.Integer) { Value = userId },
                    new OleDbParameter("@LoginTime", OleDbType.Date) { Value = DateTime.Now }
                ]);

            return await ExecuteScalarAsync<int>("SELECT @@IDENTITY");
        }

        public async Task<bool> UpdateLogoutAsync(int logId)
        {
            int rows = await ExecuteNonQueryAsync(
                "UPDATE LoginLogs SET LogoutTime = ? WHERE LogID = ?",
                [
                    new OleDbParameter("@LogoutTime", OleDbType.Date) { Value = DateTime.Now },
                    new OleDbParameter("@LogID",      OleDbType.Integer) { Value = logId }
                ]);

            return rows > 0;
        }

        public Task<List<LoginLog>> GetByUserIdAsync(int userId) =>
            ExecuteReaderAsync(
                "SELECT LogID, UserID, LoginTime, LogoutTime FROM LoginLogs WHERE UserID = ? ORDER BY LoginTime DESC",
                MapLoginLog,
                [new OleDbParameter("@UserID", userId)]);

        public Task<List<LoginLog>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null)
        {
            var query = "SELECT LogID, UserID, LoginTime, LogoutTime FROM LoginLogs";
            var parameters = new List<OleDbParameter>();
            bool hasWhere = false;

            if (fromDate.HasValue)
            {
                query += " WHERE LoginTime >= ?";
                parameters.Add(new OleDbParameter("@FromDate", OleDbType.Date) { Value = fromDate.Value });
                hasWhere = true;
            }

            if (toDate.HasValue)
            {
                query += hasWhere ? " AND LoginTime <= ?" : " WHERE LoginTime <= ?";
                parameters.Add(new OleDbParameter("@ToDate", OleDbType.Date) { Value = toDate.Value });
            }

            query += " ORDER BY LoginTime DESC";

            return ExecuteReaderAsync(query, MapLoginLog, [.. parameters]);
        }
    }
}
