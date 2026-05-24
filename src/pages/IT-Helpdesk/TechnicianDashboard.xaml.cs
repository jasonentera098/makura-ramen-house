using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;
using IT_Helpdesk.ViewModels;
using IT_Helpdesk.Views;

namespace IT_Helpdesk
{
    /// <summary>
    /// Interaction logic for TechnicianDashboard.xaml
    /// </summary>
    public partial class TechnicianDashboard : Window
    {
        private TechnicianDashboardViewModel _viewModel;

        public TechnicianDashboard()
        {
            InitializeComponent();

            // Initialize TechnicianDashboardViewModel
            _viewModel = new TechnicianDashboardViewModel();
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

        private void btnNavAssignedTickets_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("AssignedTickets");
        }

        private void btnNavUpdateTickets_Click(object sender, RoutedEventArgs e)
        {
            NavigateTo("UpdateTickets");
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
                "Dashboard" => "Technician Dashboard",
                "AssignedTickets" => "Assigned Tickets",
                "UpdateTickets" => "Update Tickets",
                "Reports" => "Reports",
                "ChangePassword" => "Change Password",
                _ => "Technician Dashboard"
            };

            // Load appropriate content view into MainContentArea
            switch (page)
            {
                case "Dashboard":
                    var dashboardContent = new TechnicianDashboardContent();
                    dashboardContent.DataContext = _viewModel;
                    MainContentArea.Content = dashboardContent;
                    break;
                case "AssignedTickets":
                    var assignedTicketsContent = new TechnicianAssignedTicketsContent();
                    var assignedTicketsViewModel = new TechnicianAssignedTicketsViewModel();
                    assignedTicketsViewModel.TicketUpdateRequested += OnTicketSelectedForUpdate;
                    assignedTicketsContent.DataContext = assignedTicketsViewModel;
                    MainContentArea.Content = assignedTicketsContent;
                    break;
                case "UpdateTickets":
                    // Show ticket selection view
                    ShowTicketUpdateSelection();
                    break;
                case "Reports":
                    var reportContent = new TechnicianReportContent();
                    var reportVm = new TechnicianReportViewModel(
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
                    MainContentArea.Content = new TextBlock
                    {
                        Text = "Technician Dashboard",
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        FontSize = 16,
                        Foreground = new SolidColorBrush(Colors.Gray)
                    };
                    break;
            }
        }

        private void ShowTicketUpdateSelection()
        {
            // Create a new ViewModel instance for ticket selection
            var selectionViewModel = new TechnicianTicketUpdateSelectionViewModel(_viewModel);
            
            // Subscribe to the ticket selected event
            selectionViewModel.TicketSelected += OnTicketSelectedForUpdate;
            
            var selectionContent = new TicketUpdateSelectionContent();
            selectionContent.DataContext = selectionViewModel;
            MainContentArea.Content = selectionContent;
        }

        private void OnTicketSelectedForUpdate(object? sender, Ticket ticket)
        {
            if (ticket == null) return;

            // Create services
            var connStr = DatabaseConfig.ConnectionString;
            var ticketRepo = new TicketRepository(connStr);
            var ticketUpdateRepo = new TicketUpdateRepository(connStr);
            var userRepo = new UserRepository(connStr);
            var attachmentRepo = new AttachmentRepository(connStr);
            var ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
            var fileService = new FileService(attachmentRepo);

            // Create ViewModel for ticket update
            var updateViewModel = new TicketUpdateViewModel(ticketService, fileService, ticket,
                ticketUpdateRepo, userRepo);
            
            // Show ticket update form
            var updateContent = new TicketUpdateContent();
            updateContent.DataContext = updateViewModel;
            MainContentArea.Content = updateContent;
        }
    }
}
