using IT_Helpdesk.Models;

namespace IT_Helpdesk.Repositories
{
    public interface ILoginLogRepository
    {
        Task<int> InsertLoginAsync(int userId);
        Task<bool> UpdateLogoutAsync(int logId);
        Task<List<LoginLog>> GetByUserIdAsync(int userId);
        Task<List<LoginLog>> GetAllAsync(DateTime? fromDate = null, DateTime? toDate = null);
    }
}
