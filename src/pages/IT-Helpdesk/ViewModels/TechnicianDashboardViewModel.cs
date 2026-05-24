using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace IT_Helpdesk.ViewModels
{
    public class TechnicianDashboardViewModel : ViewModelBase
    {
        private readonly ITicketService _ticketService;
        private readonly IAuthenticationService _authService;

        private ObservableCollection<Ticket> _assignedTickets = new();
        private int _pendingCount;
        private int _inProgressCount;
        private int _resolvedCount;
        private int _totalCount;
        private bool _isLoading;
        private Ticket? _selectedTicket;

        // Chart properties
        private ISeries[] _statusSeries = Array.Empty<ISeries>();
        private ISeries[] _categorySeries = Array.Empty<ISeries>();
        private ISeries[] _prioritySeries = Array.Empty<ISeries>();

        public TechnicianDashboardViewModel()
        {
            // Parameterless constructor: wire up services from repositories directly
            var connStr = DatabaseConfig.ConnectionString;
            var ticketRepo = new TicketRepository(connStr);
            var ticketUpdateRepo = new TicketUpdateRepository(connStr);
            var userRepo = new UserRepository(connStr);
            var loginLogRepo = new LoginLogRepository(connStr);

            _ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
            _authService = new AuthenticationService(userRepo, loginLogRepo);

            InitializeCommands();
            _ = LoadTicketsAsync();
        }

        public TechnicianDashboardViewModel(ITicketService ticketService, IAuthenticationService authService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            InitializeCommands();
            _ = LoadTicketsAsync();
        }

        private void InitializeCommands()
        {
            _assignedTickets = new ObservableCollection<Ticket>();

            UpdateTicketCommand = new RelayCommand<Ticket>(NavigateToUpdateTicket);
            ViewTicketCommand = new RelayCommand<Ticket>(ViewTicketDetails);
            RefreshCommand = new RelayCommand(async () => await LoadTicketsAsync());
            LogoutCommand = new RelayCommand(async () => await LogoutAsync());
        }

        #region Properties

        public ObservableCollection<Ticket> AssignedTickets
        {
            get => _assignedTickets;
            set => SetProperty(ref _assignedTickets, value);
        }

        public int PendingCount
        {
            get => _pendingCount;
            set => SetProperty(ref _pendingCount, value);
        }

        public int InProgressCount
        {
            get => _inProgressCount;
            set => SetProperty(ref _inProgressCount, value);
        }

        public int ResolvedCount
        {
            get => _resolvedCount;
            set => SetProperty(ref _resolvedCount, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public Ticket? SelectedTicket
        {
            get => _selectedTicket;
            set => SetProperty(ref _selectedTicket, value);
        }

        public ISeries[] StatusSeries
        {
            get => _statusSeries;
            set => SetProperty(ref _statusSeries, value);
        }

        public ISeries[] CategorySeries
        {
            get => _categorySeries;
            set => SetProperty(ref _categorySeries, value);
        }

        public ISeries[] PrioritySeries
        {
            get => _prioritySeries;
            set => SetProperty(ref _prioritySeries, value);
        }

        #endregion

        #region Commands

        public ICommand UpdateTicketCommand { get; private set; } = null!;
        public ICommand ViewTicketCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand LogoutCommand { get; private set; } = null!;

        #endregion

        #region Data Loading

        private async Task LoadTicketsAsync()
        {
            try
            {
                IsLoading = true;

                // Get current user from session
                if (SessionManager.CurrentUser == null)
                {
                    MessageBox.Show("No user session found. Please log in again.",
                        "Session Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Load tickets assigned to the current technician
                var tickets = await _ticketService.GetTicketsByUserAsync(SessionManager.CurrentUser.UserID, "Technician");
                AssignedTickets = new ObservableCollection<Ticket>(tickets);

                // Calculate summary counts
                PendingCount = tickets.Count(t => t.Status == "Pending");
                InProgressCount = tickets.Count(t => t.Status == "In Progress");
                ResolvedCount = tickets.Count(t => t.Status == "Resolved");
                TotalCount = tickets.Count;

                // Generate chart data
                GenerateStatusChart(tickets);
                GenerateCategoryChart(tickets);
                GeneratePriorityChart(tickets);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading tickets: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void GenerateStatusChart(System.Collections.Generic.List<Ticket> tickets)
        {
            var pending = tickets.Count(t => t.Status == "Pending");
            var inProgress = tickets.Count(t => t.Status == "In Progress");
            var resolved = tickets.Count(t => t.Status == "Resolved");
            var closed = tickets.Count(t => t.Status == "Closed");

            StatusSeries = new ISeries[]
            {
                new PieSeries<int>
                {
                    Values = new[] { pending },
                    Name = "Pending",
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 16,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                    DataLabelsFormatter = point => $"{pending}",
                    Fill = new SolidColorPaint(SKColor.Parse("#9E9E9E")),
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 }
                },
                new PieSeries<int>
                {
                    Values = new[] { inProgress },
                    Name = "In Progress",
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 16,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                    DataLabelsFormatter = point => $"{inProgress}",
                    Fill = new SolidColorPaint(SKColor.Parse("#007ACC")),
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 }
                },
                new PieSeries<int>
                {
                    Values = new[] { resolved },
                    Name = "Resolved",
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 16,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                    DataLabelsFormatter = point => $"{resolved}",
                    Fill = new SolidColorPaint(SKColor.Parse("#28A745")),
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 }
                },
                new PieSeries<int>
                {
                    Values = new[] { closed },
                    Name = "Closed",
                    DataLabelsPaint = new SolidColorPaint(SKColors.White),
                    DataLabelsSize = 16,
                    DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                    DataLabelsFormatter = point => $"{closed}",
                    Fill = new SolidColorPaint(SKColor.Parse("#3D3D3D")),
                    Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 }
                }
            };
        }

        private void GenerateCategoryChart(System.Collections.Generic.List<Ticket> tickets)
        {
            var categoryGroups = tickets.GroupBy(t => t.Category)
                .Select(g => new { Category = g.Key, Count = g.Count() })
                .ToList();

            var colors = new[]
            {
                SKColor.Parse("#007ACC"), // Blue
                SKColor.Parse("#28A745"), // Green
                SKColor.Parse("#FD7E14"), // Orange
                SKColor.Parse("#DC3545"), // Red
                SKColor.Parse("#6C757D"), // Gray
                SKColor.Parse("#17A2B8")  // Cyan
            };

            CategorySeries = categoryGroups.Select((g, index) => new PieSeries<int>
            {
                Values = new[] { g.Count },
                Name = g.Category,
                DataLabelsPaint = new SolidColorPaint(SKColors.White),
                DataLabelsSize = 16,
                DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Outer,
                DataLabelsFormatter = point => $"{g.Count}",
                Fill = new SolidColorPaint(colors[index % colors.Length]),
                Stroke = new SolidColorPaint(SKColors.White) { StrokeThickness = 2 }
            }).ToArray<ISeries>();
        }

        private void GeneratePriorityChart(System.Collections.Generic.List<Ticket> tickets)
        {
            var high = tickets.Count(t => t.Priority == "High");
            var medium = tickets.Count(t => t.Priority == "Medium");
            var low = tickets.Count(t => t.Priority == "Low");

            PrioritySeries = new ISeries[]
            {
                new ColumnSeries<int>
                {
                    Values = new[] { high },
                    Name = "High",
                    Fill = new SolidColorPaint(SKColor.Parse("#DC3545")),
                    Stroke = new SolidColorPaint(SKColor.Parse("#DC3545")) { StrokeThickness = 0 },
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 16,
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsFormatter = point => $"{high}"
                },
                new ColumnSeries<int>
                {
                    Values = new[] { medium },
                    Name = "Medium",
                    Fill = new SolidColorPaint(SKColor.Parse("#FD7E14")),
                    Stroke = new SolidColorPaint(SKColor.Parse("#FD7E14")) { StrokeThickness = 0 },
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 16,
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsFormatter = point => $"{medium}"
                },
                new ColumnSeries<int>
                {
                    Values = new[] { low },
                    Name = "Low",
                    Fill = new SolidColorPaint(SKColor.Parse("#28A745")),
                    Stroke = new SolidColorPaint(SKColor.Parse("#28A745")) { StrokeThickness = 0 },
                    DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                    DataLabelsSize = 16,
                    DataLabelsPosition = LiveChartsCore.Measure.DataLabelsPosition.Top,
                    DataLabelsFormatter = point => $"{low}"
                }
            };
        }

        #endregion

        #region Navigation

        private void NavigateToUpdateTicket(Ticket ticket)
        {
            if (ticket == null) return;

            // Navigate to ticket update page
            // This will be implemented in Task 14
            MessageBox.Show($"Update Ticket functionality will be implemented in Task 14\nTicket #{ticket.TicketID}: {ticket.Title}", 
                "Info", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewTicketDetails(Ticket ticket)
        {
            if (ticket == null) return;

            // Navigate to ticket details view
            // This will be implemented in subsequent tasks
            MessageBox.Show($"View details for Ticket #{ticket.TicketID}: {ticket.Title}", 
                "Ticket Details", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private async Task LogoutAsync()
        {
            try
            {
                // Update LoginLog LogoutTime and clear session
                if (SessionManager.CurrentUser != null)
                    await _authService.LogoutAsync(SessionManager.CurrentUser.UserID);

                // Open login window
                var loginWindow = new MainWindow();
                var userRepo = new UserRepository(DatabaseConfig.ConnectionString);
                var loginLogRepo = new LoginLogRepository(DatabaseConfig.ConnectionString);
                var authService = new AuthenticationService(userRepo, loginLogRepo);
                loginWindow.DataContext = new LoginViewModel(authService);
                loginWindow.Show();

                // Close the technician dashboard window
                foreach (Window win in Application.Current.Windows)
                {
                    if (win is TechnicianDashboard)
                    {
                        win.Close();
                        break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Logout failed: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #endregion
    }
}
