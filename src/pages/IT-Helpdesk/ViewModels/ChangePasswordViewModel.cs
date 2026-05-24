using System;
using System.Windows;
using IT_Helpdesk.Helpers;
using IT_Helpdesk.Services;

namespace IT_Helpdesk.ViewModels
{
    /// <summary>
    /// ViewModel for the Change Password feature (Requirements 20.1–20.8).
    /// Uses SessionManager.CurrentUser.UserID so only the logged-in user's
    /// own password can be changed (Requirement 20.8).
    /// </summary>
    public class ChangePasswordViewModel : ViewModelBase
    {
        private readonly IUserService _userService;

        private string _errorMessage = string.Empty;
        private bool _isSaving;

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsSaving
        {
            get => _isSaving;
            set => SetProperty(ref _isSaving, value);
        }

        /// <summary>
        /// Raised after a successful password change so the code-behind can clear
        /// the PasswordBox controls (which are not data-bindable).
        /// </summary>
        public event Action? PasswordChangedSuccessfully;

        public RelayCommand SaveCommand { get; }

        public ChangePasswordViewModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            // SaveCommand is wired via the XAML button's Click handler calling ExecuteSave,
            // but we expose a RelayCommand as well for completeness / testability.
            SaveCommand = new RelayCommand(() => { /* triggered via ExecuteSave from code-behind */ });
        }

        /// <summary>
        /// Called by the code-behind's button Click handler, passing the PasswordBox values
        /// that cannot be data-bound (Requirement 25.3 / 25.4).
        /// </summary>
        public async void ExecuteSave(string currentPassword, string newPassword, string confirmPassword)
        {
            // Clear previous error
            ErrorMessage = string.Empty;

            // --- Client-side validation (Requirement 20.4 / 20.7) ---
            if (string.IsNullOrWhiteSpace(currentPassword))
            {
                ErrorMessage = "Current password is required.";
                return;
            }

            // Validate new password strength
            var passwordCheck = ValidationHelper.IsStrongPassword(newPassword);
            if (!passwordCheck.IsValid)
            {
                ErrorMessage = passwordCheck.ErrorMessage;
                return;
            }

            if (newPassword != confirmPassword)
            {
                ErrorMessage = "New password and confirmation do not match.";
                return;
            }

            IsSaving = true;
            try
            {
                // Requirement 20.8: always use the currently logged-in user's ID
                int userId = IT_Helpdesk.SessionManager.CurrentUser?.UserID
                    ?? throw new InvalidOperationException("No user is currently logged in.");

                bool success = await _userService.ChangePasswordAsync(userId, currentPassword, newPassword);

                if (!success)
                {
                    // Service returns false when current password verification fails
                    ErrorMessage = "Current password is incorrect.";
                    return;
                }

                // Requirement 20.6: show success message
                MessageBox.Show(
                    "Password changed successfully!",
                    "Success",
                    MessageBoxButton.OK,
                    MessageBoxImage.Information);

                // Notify code-behind to clear PasswordBox controls
                PasswordChangedSuccessfully?.Invoke();
            }
            catch (Exception ex)
            {
                ErrorMessage = $"An error occurred: {ex.Message}";
            }
            finally
            {
                IsSaving = false;
            }
        }
    }
}
