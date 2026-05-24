using System.Data.OleDb;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk
{
    public class UserRepository : RepositoryBase, IUserRepository
    {
        public UserRepository(string connectionString) : base(connectionString) { }

        private static User MapUser(OleDbDataReader reader) => new User
        {
            UserID        = reader.GetInt32(reader.GetOrdinal("UserID")),
            FullName      = reader["FullName"]?.ToString() ?? string.Empty,
            Username      = reader["Username"]?.ToString() ?? string.Empty,
            PasswordHash  = reader["PasswordHash"]?.ToString() ?? string.Empty,
            Role          = reader["Role"]?.ToString() ?? string.Empty,
            DepartmentID  = reader["DepartmentID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["DepartmentID"]),
            ContactNumber = reader["ContactNumber"]?.ToString() ?? string.Empty,
            AccountStatus = reader["AccountStatus"]?.ToString() ?? "Active",
            CreatedAt     = reader["CreatedAt"] == DBNull.Value
                                ? DateTime.MinValue
                                : Convert.ToDateTime(reader["CreatedAt"])
        };

        private const string SelectColumns = "UserID, FullName, Username, PasswordHash, Role, DepartmentID, ContactNumber, AccountStatus, CreatedAt";

        public Task<User?> GetByUsernameAsync(string username) =>
            ExecuteReaderAsync(
                $"SELECT {SelectColumns} FROM Users WHERE Username = ?",
                MapUser,
                [new OleDbParameter("@Username", username)])
            .ContinueWith(t => t.Result.FirstOrDefault());

        public Task<User?> GetByIdAsync(int userId) =>
            ExecuteReaderAsync(
                $"SELECT {SelectColumns} FROM Users WHERE UserID = ?",
                MapUser,
                [new OleDbParameter("@UserID", OleDbType.Integer) { Value = userId }])
            .ContinueWith(t => t.Result.FirstOrDefault());

        public Task<List<User>> GetAllAsync() =>
            ExecuteReaderAsync(
                $"SELECT {SelectColumns} FROM Users ORDER BY FullName",
                MapUser);

        public Task<List<User>> GetByRoleAsync(string role) =>
            ExecuteReaderAsync(
                $"SELECT {SelectColumns} FROM Users WHERE Role = ? ORDER BY FullName",
                MapUser,
                [new OleDbParameter("@Role", role)]);

        public Task<List<User>> GetActiveByRoleAsync(string role) =>
            ExecuteReaderAsync(
                $"SELECT {SelectColumns} FROM Users WHERE Role = ? AND AccountStatus = ? ORDER BY FullName",
                MapUser,
                [
                    new OleDbParameter("@Role", role),
                    new OleDbParameter("@AccountStatus", "Active")
                ]);

        public async Task<int> InsertAsync(User user)
        {
            await ExecuteNonQueryAsync(
                "INSERT INTO Users (FullName, Username, PasswordHash, Role, DepartmentID, ContactNumber, AccountStatus, CreatedAt) " +
                "VALUES (?, ?, ?, ?, ?, ?, ?, ?)",
                [
                    new OleDbParameter("@FullName",      OleDbType.VarWChar) { Value = user.FullName },
                    new OleDbParameter("@Username",      OleDbType.VarWChar) { Value = user.Username },
                    new OleDbParameter("@PasswordHash",  OleDbType.VarWChar) { Value = user.PasswordHash },
                    new OleDbParameter("@Role",          OleDbType.VarWChar) { Value = user.Role },
                    new OleDbParameter("@DepartmentID",  OleDbType.Integer) { Value = user.DepartmentID },
                    new OleDbParameter("@ContactNumber", OleDbType.VarWChar) { Value = (object?)user.ContactNumber ?? DBNull.Value },
                    new OleDbParameter("@AccountStatus", OleDbType.VarWChar) { Value = user.AccountStatus },
                    new OleDbParameter("@CreatedAt",     OleDbType.Date) { Value = user.CreatedAt }
                ]);

            return await ExecuteScalarAsync<int>("SELECT @@IDENTITY");
        }

        public async Task<bool> UpdateAsync(User user)
        {
            int rows = await ExecuteNonQueryAsync(
                "UPDATE Users SET FullName = ?, Username = ?, PasswordHash = ?, Role = ?, " +
                "DepartmentID = ?, ContactNumber = ?, AccountStatus = ? WHERE UserID = ?",
                [
                    new OleDbParameter("@FullName",      OleDbType.VarWChar) { Value = user.FullName },
                    new OleDbParameter("@Username",      OleDbType.VarWChar) { Value = user.Username },
                    new OleDbParameter("@PasswordHash",  OleDbType.VarWChar) { Value = user.PasswordHash },
                    new OleDbParameter("@Role",          OleDbType.VarWChar) { Value = user.Role },
                    new OleDbParameter("@DepartmentID",  OleDbType.Integer) { Value = user.DepartmentID },
                    new OleDbParameter("@ContactNumber", OleDbType.VarWChar) { Value = (object?)user.ContactNumber ?? DBNull.Value },
                    new OleDbParameter("@AccountStatus", OleDbType.VarWChar) { Value = user.AccountStatus },
                    new OleDbParameter("@UserID",        OleDbType.Integer) { Value = user.UserID }
                ]);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int userId)
        {
            int rows = await ExecuteNonQueryAsync(
                "DELETE FROM Users WHERE UserID = ?",
                [new OleDbParameter("@UserID", OleDbType.Integer) { Value = userId }]);
            return rows > 0;
        }

        public async Task<bool> UsernameExistsAsync(string username)
        {
            int count = await ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE Username = ?",
                [new OleDbParameter("@Username", username)]);
            return count > 0;
        }

        public async Task<bool> UsernameExistsAsync(string username, int excludeUserId)
        {
            int count = await ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE Username = ? AND UserID <> ?",
                [
                    new OleDbParameter("@Username", OleDbType.VarWChar) { Value = username },
                    new OleDbParameter("@UserID", OleDbType.Integer) { Value = excludeUserId }
                ]);
            return count > 0;
        }

        public async Task<int> CountActiveAdminsAsync()
        {
            return await ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE Role = ? AND AccountStatus = ?",
                [
                    new OleDbParameter("@Role", OleDbType.VarWChar) { Value = "Administrator" },
                    new OleDbParameter("@AccountStatus", OleDbType.VarWChar) { Value = "Active" }
                ]);
        }
    }
}
