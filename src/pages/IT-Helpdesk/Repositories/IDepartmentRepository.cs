using IT_Helpdesk.Models;

namespace IT_Helpdesk.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();
        Task<Department?> GetByIdAsync(int departmentId);
        Task<int> InsertAsync(Department department);
        Task<bool> UpdateAsync(Department department);
        Task<bool> DeleteAsync(int departmentId);
        Task<bool> HasDependenciesAsync(int departmentId);
    }
}
