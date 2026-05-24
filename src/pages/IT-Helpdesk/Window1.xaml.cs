using System.Windows;
using IT_Helpdesk.ViewModels;
using IT_Helpdesk.Views;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;

namespace IT_Helpdesk
{
    /// <summary>
    /// Interaction logic for Window1.xaml — Admin Dashboard shell.
    /// </summary>
    public partial class Window1 : Window
    {
        private AdminDashboardContent? _dashboardContent;
        private AdminTicketManagementContent? _ticketContent;
        private AdminUserManagementContent? _userContent;

        public Window1()
        {
            InitializeComponent();

            // Set ViewModel as DataContext so LogoutCommand binding on btnLogOut works
            var vm = new AdminDashboardViewModel();
            DataContext = vm;

            // Subscribe to navigation requests from the dashboard ViewModel so that
            // commands like "Manage Users" or "Assign Ticket" navigate within this window
            // rather than opening separate windows, keeping the session context intact.
            vm.NavigationRequested += OnDashboardNavigationRequested;

            // Show the logged-in user's name in the header
            if (SessionManager.CurrentUser != null)
                txtWelcome.Text = $"Welcome, {SessionManager.CurrentUser.FullName}";

            // Wire up sidebar nav events
            btnNavDashboard.Checked += (s, e) => NavigateToDashboard();
            btnNavTickets.Checked += (s, e) => NavigateToTickets();
            btnNavUsers.Checked += (s, e) => NavigateToUsers();
            btnNavReports.Checked += (s, e) => NavigateToReports();
            btnNavChangePassword.Checked += (s, e) => NavigateToChangePassword();

            // Initialize with dashboard view
            NavigateToDashboard();
        }

        /// <summary>
        /// Handles navigation requests raised by <see cref="AdminDashboardViewModel"/> so that
        /// in-app navigation (e.g. "Go to Tickets", "Go to Users") is performed within this
        /// window while the current <see cref="SessionManager.CurrentUser"/> context is preserved.
        /// </summary>
        private void OnDashboardNavigationRequested(object? sender, string section)
        {
            switch (section)
            {
                case "Tickets":
                    btnNavTickets.IsChecked = true;
                    NavigateToTickets();
                    break;
                case "Users":
                    btnNavUsers.IsChecked = true;
                    NavigateToUsers();
                    break;
                case "Reports":
                    btnNavReports.IsChecked = true;
                    NavigateToReports();
                    break;
                default:
                    NavigateToDashboard();
                    break;
            }
        }

        private void NavigateToDashboard()
        {
            txtPageTitle.Text = "Admin Dashboard";

            if (_dashboardContent == null)
            {
                _dashboardContent = new AdminDashboardContent();
                _dashboardContent.DataContext = DataContext; // reuse existing AdminDashboardViewModel
            }

            MainContentArea.Content = _dashboardContent;
        }

        private void NavigateToTickets()
        {
            txtPageTitle.Text = "Ticket Management";

            if (_ticketContent == null)
            {
                var vm = new AdminTicketManagementViewModel();
                _ticketContent = new AdminTicketManagementContent();
                _ticketContent.DataContext = vm;
            }

            MainContentArea.Content = _ticketContent;
        }

        private void NavigateToUsers()
        {
            txtPageTitle.Text = "User Management";

            if (_userContent == null)
            {
                var vm = new AdminUserManagementViewModel();
                _userContent = new AdminUserManagementContent();
                _userContent.DataContext = vm;
            }

            MainContentArea.Content = _userContent;
        }

        private void NavigateToReports()
        {
            txtPageTitle.Text = "Reports";

            var reportContent = new AdminReportContent();
            var reportVm = new AdminReportViewModel(
                new ReportService(
                    new TicketRepository(DatabaseConfig.ConnectionString),
                    new UserRepository(DatabaseConfig.ConnectionString)),
                new UserService(
                    new UserRepository(DatabaseConfig.ConnectionString),
                    new TicketRepository(DatabaseConfig.ConnectionString)),
                new DepartmentService(
                    new DepartmentRepository(DatabaseConfig.ConnectionString)));
            reportVm.CloseRequested += (s, e) => NavigateToDashboard();
            reportContent.DataContext = reportVm;
            MainContentArea.Content = reportContent;
        }

        private void NavigateToChangePassword()
        {
            txtPageTitle.Text = "Change Password";

            var changePasswordContent = new ChangePasswordContent();
            var changePasswordVm = new ChangePasswordViewModel(
                new UserService(
                    new UserRepository(DatabaseConfig.ConnectionString),
                    new TicketRepository(DatabaseConfig.ConnectionString)));
            changePasswordContent.DataContext = changePasswordVm;
            MainContentArea.Content = changePasswordContent;
        }
    }
}
