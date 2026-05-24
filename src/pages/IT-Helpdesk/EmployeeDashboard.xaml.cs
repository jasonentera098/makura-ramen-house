using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using IT_Helpdesk.Views;
using IT_Helpdesk.ViewModels;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;

namespace IT_Helpdesk
{
    /// <summary>
    /// Interaction logic for EmployeeDashboard.xaml
    /// </summary>
    public partial class EmployeeDashboard : Window
    {
        private EmployeeDashboardViewModel _viewModel;

        public EmployeeDashboard()
        {
            InitializeComponent();
            
            // Initialize ViewModel and set as DataContext
            _viewModel = new EmployeeDashboardViewModel();
            DataContext = _viewModel;
            
            // Set welcome message with current user name
            if (SessionManager.CurrentUser != null)
                txtWelcome.Text = $"Welcome, {SessionManager.CurrentUser.FullName}";
            
            // Load default view (Dashboard)
            NavigateTo("Dashboard");
        }

        private void btnNavDashboard_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("Dashboard");
        }

        private void btnNavMyTickets_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("MyTickets");
        }

        private void btnNavSubmitTicket_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("SubmitTicket");
        }

        private void btnNavReports_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("Reports");
        }

        private void btnNavChangePassword_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("ChangePassword");
        }

        private void NavigateTo(string page)
        {
            // Update page title
            txtPageTitle.Text = page switch
            {
                "Dashboard" => "Employee Dashboard",
                "MyTickets" => "My Tickets",
                "SubmitTicket" => "Submit Ticket",
                "Reports" => "Reports",
                "ChangePassword" => "Change Password",
                _ => "Employee Dashboard"
            };

            // Load appropriate content view into MainContentArea
            switch (page)
            {
                case "Dashboard":
                    // Create content with shared ViewModel
                    var dashboardContent = new EmployeeDashboardContent();
                    dashboardContent.DataContext = _viewModel;
                    MainContentArea.Content = dashboardContent;
                    break;
                case "MyTickets":
                    var myTicketsContent = new EmployeeMyTicketsContent();
                    MainContentArea.Content = myTicketsContent;
                    break;
                case "SubmitTicket":
                    var submitTicketContent = new TicketSubmitContent();
                    // Create TicketSubmitViewModel with required dependencies
                    var ticketService = new TicketService(
                        new TicketRepository(DatabaseConfig.ConnectionString),
                        new TicketUpdateRepository(DatabaseConfig.ConnectionString),
                        new UserRepository(DatabaseConfig.ConnectionString)
                    );
                    var attachmentRepository = new AttachmentRepository(DatabaseConfig.ConnectionString);
                    var fileService = new FileService(attachmentRepository);
                    var ticketSubmitViewModel = new TicketSubmitViewModel(
                        ticketService,
                        fileService
                    );
                    submitTicketContent.DataContext = ticketSubmitViewModel;
                    MainContentArea.Content = submitTicketContent;
                    break;
                case "Reports":
                    var reportContent = new EmployeeReportContent();
                    var reportVm = new EmployeeReportViewModel(
                        new ReportService(
                            new TicketRepository(DatabaseConfig.ConnectionString),
                            new UserRepository(DatabaseConfig.ConnectionString)),
                        new UserService(
                            new UserRepository(DatabaseConfig.ConnectionString),
                            new TicketRepository(DatabaseConfig.ConnectionString)));
                    reportVm.CloseRequested += (s, e) => NavigateTo("Dashboard");
                    reportContent.DataContext = reportVm;
                    MainContentArea.Content = reportContent;
                    break;
                case "ChangePassword":
                    var changePasswordContent = new ChangePasswordContent();
                    var changePasswordVm = new ChangePasswordViewModel(
                        new UserService(
                            new UserRepository(DatabaseConfig.ConnectionString),
                            new TicketRepository(DatabaseConfig.ConnectionString)));
                    changePasswordContent.DataContext = changePasswordVm;
                    MainContentArea.Content = changePasswordContent;
                    break;
                default:
                    var defaultContent = new EmployeeDashboardContent();
                    defaultContent.DataContext = _viewModel;
                    MainContentArea.Content = defaultContent;
                    break;
            }
        }
    }
}
