using IT_Helpdesk.Models;

namespace IT_Helpdesk.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(int userId);
        Task<List<User>> GetAllAsync();
        Task<List<User>> GetByRoleAsync(string role);
        Task<List<User>> GetActiveByRoleAsync(string role);
        Task<int> InsertAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> UsernameExistsAsync(string username, int excludeUserId);
        Task<int> CountActiveAdminsAsync();
    }
}
