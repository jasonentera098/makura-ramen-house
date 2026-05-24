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
    public class AdminDashboardViewModel : ViewModelBase
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly IAuthenticationService _authService;

        private ObservableCollection<Ticket> _allTickets = new();
        private ObservableCollection<Ticket> _recentTickets = new();
        private ObservableCollection<User> _allUsers = new();
        private int _totalTickets;
        private int _pendingTickets;
        private int _resolvedTickets;
        private bool _isLoading;

        // Chart properties
        private ISeries[] _categorySeries = Array.Empty<ISeries>();
        private ISeries[] _statusSeries = Array.Empty<ISeries>();
        private ISeries[] _prioritySeries = Array.Empty<ISeries>();

        public AdminDashboardViewModel()
        {
            // Parameterless constructor: wire up services from repositories directly
            var connStr = DatabaseConfig.ConnectionString;
            var ticketRepo = new TicketRepository(connStr);
            var ticketUpdateRepo = new TicketUpdateRepository(connStr);
            var userRepo = new UserRepository(connStr);
            var loginLogRepo = new LoginLogRepository(connStr);

            _ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
            _userService = new UserService(userRepo, ticketRepo);
            _authService = new AuthenticationService(userRepo, loginLogRepo);

            InitializeCommands();
            _ = LoadDataAsync();
        }

        public AdminDashboardViewModel(ITicketService ticketService, IUserService userService, IAuthenticationService authService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            _authService = authService ?? throw new ArgumentNullException(nameof(authService));

            InitializeCommands();
            _ = LoadDataAsync();
        }

        private void InitializeCommands()
        {
            _allTickets = new ObservableCollection<Ticket>();
            _recentTickets = new ObservableCollection<Ticket>();
            _allUsers = new ObservableCollection<User>();

            AssignTicketCommand = new RelayCommand<Ticket>(NavigateToAssignTicket);
            ManageUsersCommand = new RelayCommand(NavigateToUserManagement);
            GenerateReportCommand = new RelayCommand(NavigateToReports);
            RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
            LogoutCommand = new RelayCommand(async () => await LogoutAsync());
        }

        #region Properties

        public ObservableCollection<Ticket> AllTickets
        {
            get => _allTickets;
            set => SetProperty(ref _allTickets, value);
        }

        public ObservableCollection<Ticket> RecentTickets
        {
            get => _recentTickets;
            set => SetProperty(ref _recentTickets, value);
        }

        public ObservableCollection<User> AllUsers
        {
            get => _allUsers;
            set => SetProperty(ref _allUsers, value);
        }

        public int TotalTickets
        {
            get => _totalTickets;
            set => SetProperty(ref _totalTickets, value);
        }

        public int PendingTickets
        {
            get => _pendingTickets;
            set => SetProperty(ref _pendingTickets, value);
        }

        public int ResolvedTickets
        {
            get => _resolvedTickets;
            set => SetProperty(ref _resolvedTickets, value);
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ISeries[] CategorySeries
        {
            get => _categorySeries;
            set => SetProperty(ref _categorySeries, value);
        }

        public ISeries[] StatusSeries
        {
            get => _statusSeries;
            set => SetProperty(ref _statusSeries, value);
        }

        public ISeries[] PrioritySeries
        {
            get => _prioritySeries;
            set => SetProperty(ref _prioritySeries, value);
        }

        #endregion

        #region Commands

        public ICommand AssignTicketCommand { get; private set; } = null!;
        public ICommand ManageUsersCommand { get; private set; } = null!;
        public ICommand GenerateReportCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand LogoutCommand { get; private set; } = null!;

        #endregion

        #region Data Loading

        private async Task LoadDataAsync()
        {
            try
            {
                IsLoading = true;

                // Load all tickets (Administrator role returns all tickets)
                var tickets = await _ticketService.GetTicketsByUserAsync(0, "Administrator");
                AllTickets = new ObservableCollection<Ticket>(tickets);

                // Recent tickets: top 10 by date descending
                var recent = tickets.OrderByDescending(t => t.DateSubmitted).Take(10).ToList();
                RecentTickets = new ObservableCollection<Ticket>(recent);

                // Load all users
                var users = await _userService.GetAllUsersAsync();
                AllUsers = new ObservableCollection<User>(users);

                // Calculate summary counts
                TotalTickets = tickets.Count;
                PendingTickets = tickets.Count(t => t.Status == "Pending" || t.Status == "In Progress");
                ResolvedTickets = tickets.Count(t => t.Status == "Resolved" || t.Status == "Closed");

                // Generate chart data
                GenerateCategoryChart(tickets);
                GenerateStatusChart(tickets);
                GeneratePriorityChart(tickets);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading dashboard data: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
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

        /// <summary>
        /// Raised when the ViewModel requests navigation to a named section within the host window.
        /// The string argument is the section name (e.g. "Tickets", "Users").
        /// </summary>
        public event EventHandler<string>? NavigationRequested;

        #region Navigation

        private void NavigateToAssignTicket(Ticket ticket)
        {
            // Request the host window (Window1) to navigate to the Tickets section so the
            // current session context is preserved and no extra window is opened.
            NavigationRequested?.Invoke(this, "Tickets");
        }

        private void NavigateToUserManagement()
        {
            // Request the host window (Window1) to navigate to the Users section so the
            // current session context is preserved and no extra window is opened.
            NavigationRequested?.Invoke(this, "Users");
        }

        private void NavigateToReports()
        {
            // Navigate to Reports window
            foreach (Window win in Application.Current.Windows)
            {
                if (win.GetType().Name == "Reports" || win.GetType().Name == "Reportss")
                {
                    win.Activate();
                    return;
                }
            }

            // Try to open Reports window by type name
            try
            {
                var reportsType = Type.GetType("IT_Helpdesk.Reports") ?? Type.GetType("IT_Helpdesk.Reportss");
                if (reportsType != null)
                {
                    var reportsWindow = (Window)Activator.CreateInstance(reportsType);
                    reportsWindow?.Show();
                }
            }
            catch
            {
                MessageBox.Show("Reports module is not yet available.", "Navigation",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
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

                // Close the admin dashboard window
                foreach (Window win in Application.Current.Windows)
                {
                    if (win is Window1)
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
