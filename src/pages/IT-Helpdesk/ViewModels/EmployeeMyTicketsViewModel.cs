using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using IT_Helpdesk.Models;
using IT_Helpdesk.Repositories;
using IT_Helpdesk.Services;
using IT_Helpdesk.Views;

namespace IT_Helpdesk.ViewModels
{
    public class EmployeeMyTicketsViewModel : ViewModelBase
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        private List<TicketDisplayItem> _allTickets = new();

        private string _searchText = string.Empty;
        private string _selectedStatusFilter = "All";
        private string _selectedPriorityFilter = "All";
        private string _selectedCategoryFilter = "All";
        private ObservableCollection<TicketDisplayItem> _filteredTickets = new();
        private TicketDisplayItem? _selectedTicket;
        private bool _isLoading;
        private int _allCount;
        private int _pendingCount;
        private int _inProgressCount;
        private int _resolvedCount;
        private string _errorMessage = string.Empty;

        public EmployeeMyTicketsViewModel()
        {
            var connStr = DatabaseConfig.ConnectionString;
            var ticketRepo = new TicketRepository(connStr);
            var ticketUpdateRepo = new TicketUpdateRepository(connStr);
            var userRepo = new UserRepository(connStr);

            _ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
            _userService = new UserService(userRepo, ticketRepo);

            SearchCommand = new RelayCommand(() => ApplyFilters());
            ClearSearchCommand = new RelayCommand(ClearSearch);
            RefreshCommand = new RelayCommand(async () => await LoadTicketsAsync());

            ViewTicketCommand = new RelayCommand(
                () => ViewTicketDetails(),
                () => SelectedTicket != null);

            _ = LoadTicketsAsync();
        }

        public EmployeeMyTicketsViewModel(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            SearchCommand = new RelayCommand(() => ApplyFilters());
            ClearSearchCommand = new RelayCommand(ClearSearch);
            RefreshCommand = new RelayCommand(async () => await LoadTicketsAsync());

            ViewTicketCommand = new RelayCommand(
                () => ViewTicketDetails(),
                () => SelectedTicket != null);

            _ = LoadTicketsAsync();
        }

        #region Properties

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string SelectedStatusFilter
        {
            get => _selectedStatusFilter;
            set => SetProperty(ref _selectedStatusFilter, value);
        }

        public string SelectedPriorityFilter
        {
            get => _selectedPriorityFilter;
            set => SetProperty(ref _selectedPriorityFilter, value);
        }

        public string SelectedCategoryFilter
        {
            get => _selectedCategoryFilter;
            set => SetProperty(ref _selectedCategoryFilter, value);
        }

        public ObservableCollection<TicketDisplayItem> FilteredTickets
        {
            get => _filteredTickets;
            set => SetProperty(ref _filteredTickets, value);
        }

        public TicketDisplayItem? SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                SetProperty(ref _selectedTicket, value);
                if (!string.IsNullOrEmpty(ErrorMessage))
                    ErrorMessage = string.Empty;
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        public int AllCount
        {
            get => _allCount;
            set => SetProperty(ref _allCount, value);
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

        #endregion

        #region Commands

        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewTicketCommand { get; }

        #endregion

        #region Data Loading

        public async Task LoadTicketsAsync()
        {
            try
            {
                IsLoading = true;

                // Get only tickets submitted by the current user
                int currentUserId = SessionManager.CurrentUser!.UserID;
                var tickets = await _ticketService.GetTicketsByUserAsync(currentUserId, "Employee");

                var displayItems = new List<TicketDisplayItem>();
                foreach (var ticket in tickets)
                {
                    string assignedToName = "Unassigned";
                    if (ticket.AssignedTo.HasValue && ticket.AssignedTo.Value > 0)
                    {
                        var user = await _userService.GetUserByIdAsync(ticket.AssignedTo.Value);
                        if (user != null)
                            assignedToName = user.FullName;
                    }

                    displayItems.Add(new TicketDisplayItem
                    {
                        TicketID = ticket.TicketID,
                        Title = ticket.Title,
                        Description = ticket.Description,
                        Status = ticket.Status,
                        Priority = ticket.Priority,
                        Category = ticket.Category,
                        AssignedToName = assignedToName
                    });
                }

                _allTickets = displayItems;

                // Update summary counts
                AllCount = _allTickets.Count;
                PendingCount = _allTickets.Count(t => t.Status == "Pending");
                InProgressCount = _allTickets.Count(t => t.Status == "In Progress");
                ResolvedCount = _allTickets.Count(t => t.Status == "Resolved");

                ApplyFilters();
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

        public void ApplyFilters()
        {
            var filtered = _allTickets.AsEnumerable();

            // Search by TicketID (exact), Title, or Description (partial, case-insensitive)
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim();
                if (int.TryParse(term, out int id))
                {
                    filtered = filtered.Where(t => t.TicketID == id);
                }
                else
                {
                    filtered = filtered.Where(t =>
                        t.Title.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 ||
                        t.Description.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }

            // Status filter
            if (!string.IsNullOrEmpty(SelectedStatusFilter) && SelectedStatusFilter != "All")
                filtered = filtered.Where(t => t.Status == SelectedStatusFilter);

            // Priority filter
            if (!string.IsNullOrEmpty(SelectedPriorityFilter) && SelectedPriorityFilter != "All")
                filtered = filtered.Where(t => t.Priority == SelectedPriorityFilter);

            // Category filter
            if (!string.IsNullOrEmpty(SelectedCategoryFilter) && SelectedCategoryFilter != "All")
                filtered = filtered.Where(t => t.Category == SelectedCategoryFilter);

            FilteredTickets = new ObservableCollection<TicketDisplayItem>(filtered);
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            SelectedStatusFilter = "All";
            SelectedPriorityFilter = "All";
            SelectedCategoryFilter = "All";
            ApplyFilters();
        }

        private void ViewTicketDetails()
        {
            if (SelectedTicket == null) return;

            var dialog = new TicketDetailsDialog(SelectedTicket.TicketID);
            var owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (owner != null)
                dialog.Owner = owner;
            
            dialog.ShowDialog();
        }

        #endregion
    }
}
