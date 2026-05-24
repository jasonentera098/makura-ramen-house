using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;

namespace IT_Helpdesk.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITicketRepository _ticketRepository;

        public UserService(IUserRepository userRepository, ITicketRepository ticketRepository)
        {
            _userRepository = userRepository;
            _ticketRepository = ticketRepository;
        }

        /// <summary>
        /// Creates a new user, hashing the plain password before storing.
        /// Validates required fields and username uniqueness (Req 10.1, 10.2, 10.3, 25.1).
        /// </summary>
        public async Task<int> CreateUserAsync(User user, string plainPassword)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("Full name is required.", nameof(user));
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username is required.", nameof(user));
            if (string.IsNullOrWhiteSpace(plainPassword))
                throw new ArgumentException("Password is required.", nameof(plainPassword));
            if (string.IsNullOrWhiteSpace(user.Role))
                throw new ArgumentException("Role is required.", nameof(user));

            // Req 10.2: Ensure username is unique
            if (await _userRepository.UsernameExistsAsync(user.Username))
                throw new InvalidOperationException($"Username '{user.Username}' is already taken.");

            // Req 10.7 / 25.1: Hash password before storing
            user.PasswordHash = PasswordHasher.HashPassword(plainPassword);
            user.CreatedAt = DateTime.Now;

            return await _userRepository.InsertAsync(user);
        }

        /// <summary>
        /// Updates an existing user record (Req 10.4).
        /// If a new plain password is provided via PasswordHash field starting with a sentinel,
        /// callers should hash it before calling; otherwise PasswordHash is stored as-is.
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user)
        {
            if (string.IsNullOrWhiteSpace(user.FullName))
                throw new ArgumentException("Full name is required.", nameof(user));
            if (string.IsNullOrWhiteSpace(user.Username))
                throw new ArgumentException("Username is required.", nameof(user));
            if (string.IsNullOrWhiteSpace(user.Role))
                throw new ArgumentException("Role is required.", nameof(user));

            return await _userRepository.UpdateAsync(user);
        }

        /// <summary>
        /// Updates an existing user, re-hashing the password if a new plain password is supplied (Req 25.5).
        /// </summary>
        public async Task<bool> UpdateUserAsync(User user, string? newPlainPassword)
        {
            if (!string.IsNullOrWhiteSpace(newPlainPassword))
                user.PasswordHash = PasswordHasher.HashPassword(newPlainPassword);

            return await UpdateUserAsync(user);
        }

        /// <summary>
        /// Deletes a user. Prevents deletion if the user has submitted or been assigned tickets (Req 10.5, 30.6).
        /// </summary>
        public async Task<bool> DeleteUserAsync(int userId)
        {
            var submitted = await _ticketRepository.GetBySubmitterAsync(userId);
            if (submitted.Count > 0)
            {
                string ticketWord = submitted.Count == 1 ? "ticket" : "tickets";
                throw new InvalidOperationException(
                    $"Cannot delete user: they have submitted {submitted.Count} {ticketWord}. " +
                    "Please reassign or close these tickets before deleting the user.");
            }

            var assigned = await _ticketRepository.GetByAssigneeAsync(userId);
            if (assigned.Count > 0)
            {
                string ticketWord = assigned.Count == 1 ? "ticket" : "tickets";
                throw new InvalidOperationException(
                    $"Cannot delete user: they have been assigned {assigned.Count} {ticketWord}. " +
                    "Please reassign or close these tickets before deleting the user.");
            }

            return await _userRepository.DeleteAsync(userId);
        }

        /// <summary>Returns a user by ID (Req 10.6).</summary>
        public async Task<User?> GetUserByIdAsync(int userId) =>
            await _userRepository.GetByIdAsync(userId);

        /// <summary>Returns all users (Req 10.6).</summary>
        public async Task<List<User>> GetAllUsersAsync() =>
            await _userRepository.GetAllAsync();

        /// <summary>Returns all users with the given role.</summary>
        public async Task<List<User>> GetUsersByRoleAsync(string role) =>
            await _userRepository.GetByRoleAsync(role);

        /// <summary>Returns all active users with the given role.</summary>
        public async Task<List<User>> GetActiveUsersByRoleAsync(string role) =>
            await _userRepository.GetActiveByRoleAsync(role);

        /// <summary>Checks whether a username is already in use.</summary>
        public async Task<bool> UsernameExistsAsync(string username) =>
            await _userRepository.UsernameExistsAsync(username);

        /// <summary>
        /// Changes the password for the specified user after verifying the current password.
        /// Returns true on success, false if the current password is incorrect (Requirement 20.5).
        /// Only the user's own password is changed — the userId must match the session user (Req 20.8).
        /// </summary>
        public async Task<bool> ChangePasswordAsync(int userId, string currentPlainPassword, string newPlainPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            // Verify the current password against the stored hash
            if (!PasswordHasher.VerifyPassword(currentPlainPassword, user.PasswordHash))
                return false;

            // Hash the new password and persist
            user.PasswordHash = PasswordHasher.HashPassword(newPlainPassword);
            return await _userRepository.UpdateAsync(user);
        }
    }
}
