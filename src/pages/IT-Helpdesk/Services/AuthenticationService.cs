using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;
        private readonly ILoginLogRepository _loginLogRepository;

        public AuthenticationService(IUserRepository userRepository, ILoginLogRepository loginLogRepository)
        {
            _userRepository = userRepository;
            _loginLogRepository = loginLogRepository;
        }

        public User? CurrentUser => SessionManager.CurrentUser;

        public async Task<User?> AuthenticateAsync(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return null;

            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null || !PasswordHasher.VerifyPassword(password, user.PasswordHash))
                return null;

            int logId = await _loginLogRepository.InsertLoginAsync(user.UserID);
            SessionManager.StartSession(user, logId);

            return user;
        }

        public async Task<bool> LogoutAsync(int userId)
        {
            bool updated = await _loginLogRepository.UpdateLogoutAsync(SessionManager.CurrentLogID);
            SessionManager.EndSession();
            return updated;
        }

        public string HashPassword(string password) => PasswordHasher.HashPassword(password);

        public bool VerifyPassword(string password, string hash) => PasswordHasher.VerifyPassword(password, hash);
    }
}
