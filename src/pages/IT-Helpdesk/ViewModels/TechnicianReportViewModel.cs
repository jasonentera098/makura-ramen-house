using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IT_Helpdesk.Services;

namespace IT_Helpdesk.ViewModels
{
    public class AssignedTicketReportItem
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string SubmittedByName { get; set; } = string.Empty;
        public string DateSubmitted { get; set; } = string.Empty;
        public string DateResolved { get; set; } = string.Empty;
    }

    public class TechnicianReportViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;
        private readonly IUserService _userService;

        private DateTime? _fromDate;
        private DateTime? _toDate;
        private string _selectedStatus = "All";
        private ObservableCollection<AssignedTicketReportItem> _assignedTickets = new();
        private int _totalAssigned;
        private int _totalResolved;
        private int _inProgressCount;
        private double _avgResolutionDays;
        private bool _isLoading;

        public event EventHandler? PrintRequested;
        public event EventHandler? CloseRequested;

        public TechnicianReportViewModel(IReportService reportService, IUserService userService)
        {
            _reportService = reportService;
            _userService = userService;

            GenerateCommand = new RelayCommand(async () => await GenerateAsync());
            PrintCommand = new RelayCommand(() => PrintRequested?.Invoke(this, EventArgs.Empty));
            TodayCommand = new RelayCommand(SetToday);
            ThisWeekCommand = new RelayCommand(SetThisWeek);
            ThisMonthCommand = new RelayCommand(SetThisMonth);
            CloseCommand = new RelayCommand(() => CloseRequested?.Invoke(this, EventArgs.Empty));

            SetThisMonth();
            _ = GenerateAsync();
        }

        #region Properties

        public DateTime? FromDate
        {
            get => _fromDate;
            set => SetProperty(ref _fromDate, value);
        }

        public DateTime? ToDate
        {
            get => _toDate;
            set => SetProperty(ref _toDate, value);
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public ObservableCollection<AssignedTicketReportItem> AssignedTickets
        {
            get => _assignedTickets;
            set => SetProperty(ref _assignedTickets, value);
        }

        public int TotalAssigned
        {
            get => _totalAssigned;
            set => SetProperty(ref _totalAssigned, value);
        }

        public int TotalResolved
        {
            get => _totalResolved;
            set => SetProperty(ref _totalResolved, value);
        }

        public int InProgressCount
        {
            get => _inProgressCount;
            set => SetProperty(ref _inProgressCount, value);
        }

        public double AvgResolutionDays
        {
            get => _avgResolutionDays;
            set => SetProperty(ref _avgResolutionDays, value);
        }

        public string AvgResolutionDaysText => $"{AvgResolutionDays:F1} days";

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        #endregion

        #region Commands

        public ICommand GenerateCommand { get; }
        public ICommand PrintCommand { get; }
        public ICommand TodayCommand { get; }
        public ICommand ThisWeekCommand { get; }
        public ICommand ThisMonthCommand { get; }
        public ICommand CloseCommand { get; }

        #endregion

        #region Methods

        private void SetToday()
        {
            FromDate = DateTime.Today;
            ToDate = DateTime.Today;
        }

        private void SetThisWeek()
        {
            var today = DateTime.Today;
            FromDate = today.AddDays(-(int)today.DayOfWeek);
            ToDate = today;
        }

        private void SetThisMonth()
        {
            var today = DateTime.Today;
            FromDate = new DateTime(today.Year, today.Month, 1);
            ToDate = today;
        }

        private async Task GenerateAsync()
        {
            if (SessionManager.CurrentUser == null) return;

            try
            {
                IsLoading = true;

                string? status = SelectedStatus == "All" ? null : SelectedStatus;

                var tickets = await _reportService.GetTicketsByFiltersAsync(
                    FromDate, ToDate, null, status, null, null,
                    SessionManager.CurrentUser.UserID, "Technician");

                // Cache user lookups
                var userCache = new System.Collections.Generic.Dictionary<int, string>();

                var items = new ObservableCollection<AssignedTicketReportItem>();
                foreach (var t in tickets)
                {
                    string submittedByName = "Unknown";
                    if (!userCache.TryGetValue(t.SubmittedBy, out submittedByName!))
                    {
                        var user = await _userService.GetUserByIdAsync(t.SubmittedBy);
                        submittedByName = user?.FullName ?? "Unknown";
                        userCache[t.SubmittedBy] = submittedByName;
                    }

                    items.Add(new AssignedTicketReportItem
                    {
                        TicketID = t.TicketID,
                        Title = t.Title,
                        Priority = t.Priority,
                        Status = t.Status,
                        SubmittedByName = submittedByName,
                        DateSubmitted = t.DateSubmitted.ToString("MM/dd/yyyy"),
                        DateResolved = t.DateResolved.HasValue ? t.DateResolved.Value.ToString("MM/dd/yyyy") : "-"
                    });
                }

                AssignedTickets = items;

                // Compute summary counts in-memory
                TotalAssigned = tickets.Count;
                TotalResolved = tickets.Count(t => t.Status == "Resolved" || t.Status == "Closed");
                InProgressCount = tickets.Count(t => t.Status == "In Progress");

                // Avg resolution time for resolved tickets
                var resolvedTickets = tickets.Where(t => t.DateResolved.HasValue).ToList();
                if (resolvedTickets.Count > 0)
                {
                    double totalDays = resolvedTickets.Sum(t => (t.DateResolved!.Value - t.DateSubmitted).TotalDays);
                    AvgResolutionDays = totalDays / resolvedTickets.Count;
                }
                else
                {
                    AvgResolutionDays = 0;
                }

                OnPropertyChanged(nameof(AvgResolutionDaysText));
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        #endregion
    }
}
