using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepository;

        public DepartmentService(IDepartmentRepository departmentRepository)
        {
            _departmentRepository = departmentRepository;
        }

        /// <summary>
        /// Creates a new department. Validates name is not empty (Req 11.1).
        /// </summary>
        public async Task<int> CreateDepartmentAsync(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                throw new ArgumentException("Department name is required.", nameof(department));

            return await _departmentRepository.InsertAsync(department);
        }

        /// <summary>
        /// Updates an existing department. Validates name is not empty (Req 11.4).
        /// </summary>
        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            if (string.IsNullOrWhiteSpace(department.DepartmentName))
                throw new ArgumentException("Department name is required.", nameof(department));

            return await _departmentRepository.UpdateAsync(department);
        }

        /// <summary>
        /// Deletes a department. Prevents deletion if users or tickets depend on it (Req 11.5, 30.7).
        /// </summary>
        public async Task<bool> DeleteDepartmentAsync(int departmentId)
        {
            if (await _departmentRepository.HasDependenciesAsync(departmentId))
                throw new InvalidOperationException("Cannot delete department: it has associated users or tickets.");

            return await _departmentRepository.DeleteAsync(departmentId);
        }

        /// <summary>Returns all departments (Req 11.2).</summary>
        public async Task<List<Department>> GetDepartmentsAsync() =>
            await _departmentRepository.GetAllAsync();

        /// <summary>Returns a department by ID (Req 11.3).</summary>
        public async Task<Department?> GetDepartmentByIdAsync(int departmentId) =>
            await _departmentRepository.GetByIdAsync(departmentId);
    }
}
