using IT_Helpdesk.Models;

namespace IT_Helpdesk.Services
{
    public interface IUserService
    {
        Task<int> CreateUserAsync(User user, string plainPassword);
        Task<bool> UpdateUserAsync(User user);
        Task<bool> DeleteUserAsync(int userId);
        Task<User?> GetUserByIdAsync(int userId);
        Task<List<User>> GetAllUsersAsync();
        Task<List<User>> GetUsersByRoleAsync(string role);
        Task<List<User>> GetActiveUsersByRoleAsync(string role);
        Task<bool> UsernameExistsAsync(string username);

        /// <summary>
        /// Changes the password for the specified user after verifying the current password.
        /// Returns true on success, false if the current password is incorrect (Requirement 20.5).
        /// </summary>
        Task<bool> ChangePasswordAsync(int userId, string currentPlainPassword, string newPlainPassword);
    }
}
