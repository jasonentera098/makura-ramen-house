using System.Data.OleDb;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk
{
    public class DepartmentRepository : RepositoryBase, IDepartmentRepository
    {
        public DepartmentRepository(string connectionString) : base(connectionString) { }

        private static Department MapDepartment(OleDbDataReader reader) => new Department
        {
            DepartmentID   = reader.GetInt32(reader.GetOrdinal("DepartmentID")),
            DepartmentName = reader["DepartmentName"]?.ToString() ?? string.Empty,
            Description    = reader["Description"]?.ToString() ?? string.Empty
        };

        public Task<List<Department>> GetAllAsync() =>
            ExecuteReaderAsync(
                "SELECT DepartmentID, DepartmentName, Description FROM Departments ORDER BY DepartmentName",
                MapDepartment);

        public Task<Department?> GetByIdAsync(int departmentId) =>
            ExecuteReaderAsync(
                "SELECT DepartmentID, DepartmentName, Description FROM Departments WHERE DepartmentID = ?",
                MapDepartment,
                [new OleDbParameter("@DepartmentID", departmentId)])
            .ContinueWith(t => t.Result.FirstOrDefault());

        public async Task<int> InsertAsync(Department department)
        {
            await ExecuteNonQueryAsync(
                "INSERT INTO Departments (DepartmentName, Description) VALUES (?, ?)",
                [
                    new OleDbParameter("@DepartmentName", department.DepartmentName),
                    new OleDbParameter("@Description",    department.Description)
                ]);

            return await ExecuteScalarAsync<int>("SELECT @@IDENTITY");
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            int rows = await ExecuteNonQueryAsync(
                "UPDATE Departments SET DepartmentName = ?, Description = ? WHERE DepartmentID = ?",
                [
                    new OleDbParameter("@DepartmentName", department.DepartmentName),
                    new OleDbParameter("@Description",    department.Description),
                    new OleDbParameter("@DepartmentID",   department.DepartmentID)
                ]);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int departmentId)
        {
            int rows = await ExecuteNonQueryAsync(
                "DELETE FROM Departments WHERE DepartmentID = ?",
                [new OleDbParameter("@DepartmentID", departmentId)]);
            return rows > 0;
        }

        /// <summary>
        /// Returns true if any Users or Tickets are associated with this department,
        /// which would prevent safe deletion (Requirement 30.7 / Requirement 11.5).
        /// </summary>
        public async Task<bool> HasDependenciesAsync(int departmentId)
        {
            int userCount = await ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Users WHERE DepartmentID = ?",
                [new OleDbParameter("@DepartmentID", departmentId)]);

            if (userCount > 0)
                return true;

            int ticketCount = await ExecuteScalarAsync<int>(
                "SELECT COUNT(*) FROM Tickets WHERE DepartmentID = ?",
                [new OleDbParameter("@DepartmentID", departmentId)]);

            return ticketCount > 0;
        }
    }
}
