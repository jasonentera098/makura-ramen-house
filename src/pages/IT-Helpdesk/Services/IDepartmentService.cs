using IT_Helpdesk.Models;

namespace IT_Helpdesk.Services
{
    public interface IDepartmentService
    {
        Task<int> CreateDepartmentAsync(Department department);
        Task<bool> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int departmentId);
        Task<List<Department>> GetDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int departmentId);
    }
}
