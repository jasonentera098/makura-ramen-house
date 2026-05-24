using IT_Helpdesk.Models;

namespace IT_Helpdesk.Services
{
    public interface IAuthenticationService
    {
        Task<User?> AuthenticateAsync(string username, string password);
        Task<bool> LogoutAsync(int userId);
        string HashPassword(string password);
        bool VerifyPassword(string password, string hash);
        User? CurrentUser { get; }
    }
}
