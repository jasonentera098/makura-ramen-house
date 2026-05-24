using System;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using IT_Helpdesk.Services;
using static IT_Helpdesk.MainWindow;

namespace IT_Helpdesk.ViewModels
{
    public class LoginViewModel : ViewModelBase, IHasPassword
    {
        private readonly IAuthenticationService _authService;
        private string _username;
        private string _password;
        private string _errorMessage;
        private bool _isLoading;

        public string Username
        {
            get => _username;
            set
            {
                if (SetProperty(ref _username, value))
                {
                    // Clear any previous error when the user edits the field (Requirement 20.6)
                    if (!string.IsNullOrEmpty(ErrorMessage))
                        ErrorMessage = string.Empty;
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string Password
        {
            get => _password;
            set
            {
                if (SetProperty(ref _password, value))
                {
                    // Clear any previous error when the user edits the field (Requirement 20.6)
                    if (!string.IsNullOrEmpty(ErrorMessage))
                        ErrorMessage = string.Empty;
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand LoginCommand { get; }

        public LoginViewModel(IAuthenticationService authService)
        {
            _authService = authService;
            LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);
        }

        private bool CanLogin() =>
            !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);

        private async Task LoginAsync()
        {
            IsLoading = true;
            ErrorMessage = string.Empty;

            try
            {
                var user = await _authService.AuthenticateAsync(Username, Password);
                if (user != null)
                {
                    NavigateToDashboard(user.Role);
                }
                else
                {
                    ErrorMessage = "Invalid username or password";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Login failed: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void NavigateToDashboard(string role)
        {
            // Use WindowManager to prevent multiple dashboard instances of the same type.
            switch (role)
            {
                case "Employee":
                    WindowManager.ShowSingle<EmployeeDashboard>(() => new EmployeeDashboard());
                    break;
                case "Technician":
                    WindowManager.ShowSingle<TechnicianDashboard>(() => new TechnicianDashboard());
                    break;
                case "Administrator":
                    WindowManager.ShowSingle<Window1>(() => new Window1());
                    break;
                default:
                    ErrorMessage = $"Unknown role: {role}";
                    return;
            }

            // Close the login window
            foreach (System.Windows.Window win in System.Windows.Application.Current.Windows)
            {
                if (win is MainWindow)
                {
                    win.Close();
                    break;
                }
            }
        }
    }
}
