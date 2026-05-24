using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;

namespace IT_Helpdesk.ViewModels
{
    public class TicketHistoryItem
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string DateSubmitted { get; set; } = string.Empty;
        public string DateResolved { get; set; } = string.Empty;
    }

    public class EmployeeReportViewModel : ViewModelBase
    {
        private readonly IReportService _reportService;
        private readonly IUserService _userService;

        private DateTime? _fromDate;
        private DateTime? _toDate;
        private string _selectedCategory = "All";
        private string _selectedStatus = "All";
        private ObservableCollection<TicketHistoryItem> _ticketHistory = new();
        private int _totalCount;
        private int _pendingCount;
        private int _resolvedCount;
        private int _closedCount;
        private bool _isLoading;

        public event EventHandler? PrintRequested;
        public event EventHandler? CloseRequested;

        public EmployeeReportViewModel(IReportService reportService, IUserService userService)
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

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string SelectedStatus
        {
            get => _selectedStatus;
            set => SetProperty(ref _selectedStatus, value);
        }

        public ObservableCollection<TicketHistoryItem> TicketHistory
        {
            get => _ticketHistory;
            set => SetProperty(ref _ticketHistory, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
        }

        public int PendingCount
        {
            get => _pendingCount;
            set => SetProperty(ref _pendingCount, value);
        }

        public int ResolvedCount
        {
            get => _resolvedCount;
            set => SetProperty(ref _resolvedCount, value);
        }

        public int ClosedCount
        {
            get => _closedCount;
            set => SetProperty(ref _closedCount, value);
        }

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

                string? category = SelectedCategory == "All" ? null : SelectedCategory;
                string? status = SelectedStatus == "All" ? null : SelectedStatus;

                var tickets = await _reportService.GetTicketsByFiltersAsync(
                    FromDate, ToDate, category, status, null, null,
                    SessionManager.CurrentUser.UserID, "Employee");

                var items = new ObservableCollection<TicketHistoryItem>();
                foreach (var t in tickets)
                {
                    items.Add(new TicketHistoryItem
                    {
                        TicketID = t.TicketID,
                        Title = t.Title,
                        Category = t.Category,
                        Priority = t.Priority,
                        Status = t.Status,
                        DateSubmitted = t.DateSubmitted.ToString("MM/dd/yyyy"),
                        DateResolved = t.DateResolved.HasValue ? t.DateResolved.Value.ToString("MM/dd/yyyy") : "-"
                    });
                }

                TicketHistory = items;

                // Compute summary counts in-memory
                TotalCount = tickets.Count;
                PendingCount = tickets.Count(t => t.Status == "Pending" || t.Status == "In Progress");
                ResolvedCount = tickets.Count(t => t.Status == "Resolved");
                ClosedCount = tickets.Count(t => t.Status == "Closed");
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
