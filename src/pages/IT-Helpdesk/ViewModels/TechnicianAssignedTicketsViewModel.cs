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

namespace IT_Helpdesk.ViewModels
{
    /// <summary>
    /// Display item for the Assigned Tickets DataGrid, includes resolved submitter name.
    /// </summary>
    public class AssignedTicketDisplayItem
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string SubmittedByName { get; set; } = string.Empty;
        
        // Store the full ticket object for navigation
        public Ticket? FullTicket { get; set; }
    }

    public class TechnicianAssignedTicketsViewModel : ViewModelBase
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;

        // Full list loaded from DB (scoped to current technician)
        private List<AssignedTicketDisplayItem> _allTickets = new();

        private string _searchText = string.Empty;
        private string _selectedStatusFilter = "All";
        private string _selectedPriorityFilter = "All";
        private ObservableCollection<AssignedTicketDisplayItem> _assignedTickets = new();
        private AssignedTicketDisplayItem? _selectedTicket;
        private bool _isLoading;
        private int _totalCount;
        private int _inProgressCount;
        private int _resolvedCount;

        // Event to notify parent window to navigate to ticket update
        public event EventHandler<Ticket>? TicketUpdateRequested;

        public TechnicianAssignedTicketsViewModel()
        {
            var connStr = DatabaseConfig.ConnectionString;
            var ticketRepo = new TicketRepository(connStr);
            var ticketUpdateRepo = new TicketUpdateRepository(connStr);
            var userRepo = new UserRepository(connStr);

            _ticketService = new TicketService(ticketRepo, ticketUpdateRepo, userRepo);
            _userService = new UserService(userRepo, ticketRepo);

            InitializeCommands();
            _ = LoadTicketsAsync();
        }

        public TechnicianAssignedTicketsViewModel(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            InitializeCommands();
            _ = LoadTicketsAsync();
        }

        private void InitializeCommands()
        {
            SearchCommand = new RelayCommand(() => ApplyFilters());
            ClearSearchCommand = new RelayCommand(ClearSearch);
            RefreshCommand = new RelayCommand(async () => await LoadTicketsAsync());
            UpdateTicketCommand = new RelayCommand(NavigateToUpdateTicket, () => SelectedTicket != null);
        }

        private void NavigateToUpdateTicket()
        {
            if (SelectedTicket?.FullTicket != null)
            {
                TicketUpdateRequested?.Invoke(this, SelectedTicket.FullTicket);
            }
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
            set
            {
                if (SetProperty(ref _selectedStatusFilter, value))
                    ApplyFilters();
            }
        }

        public string SelectedPriorityFilter
        {
            get => _selectedPriorityFilter;
            set
            {
                if (SetProperty(ref _selectedPriorityFilter, value))
                    ApplyFilters();
            }
        }

        /// <summary>The filtered collection bound to the DataGrid.</summary>
        public ObservableCollection<AssignedTicketDisplayItem> AssignedTickets
        {
            get => _assignedTickets;
            set => SetProperty(ref _assignedTickets, value);
        }

        public AssignedTicketDisplayItem? SelectedTicket
        {
            get => _selectedTicket;
            set
            {
                SetProperty(ref _selectedTicket, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public int TotalCount
        {
            get => _totalCount;
            set => SetProperty(ref _totalCount, value);
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

        public ICommand SearchCommand { get; private set; } = null!;
        public ICommand ClearSearchCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand UpdateTicketCommand { get; private set; } = null!;

        #endregion

        #region Data Loading

        public async Task LoadTicketsAsync()
        {
            try
            {
                IsLoading = true;

                if (SessionManager.CurrentUser == null)
                {
                    MessageBox.Show("No user session found. Please log in again.",
                        "Session Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Load only tickets assigned to the current technician
                var tickets = await _ticketService.GetTicketsByUserAsync(
                    SessionManager.CurrentUser.UserID, "Technician");

                // Build display items (resolve submitter names)
                var displayItems = new List<AssignedTicketDisplayItem>();
                foreach (var ticket in tickets)
                {
                    string submittedByName = string.Empty;
                    if (ticket.SubmittedBy > 0)
                    {
                        var submitter = await _userService.GetUserByIdAsync(ticket.SubmittedBy);
                        submittedByName = submitter?.FullName ?? string.Empty;
                    }

                    displayItems.Add(new AssignedTicketDisplayItem
                    {
                        TicketID = ticket.TicketID,
                        Title = ticket.Title,
                        Status = ticket.Status,
                        Priority = ticket.Priority,
                        SubmittedByName = submittedByName,
                        FullTicket = ticket // Store the full ticket for navigation
                    });
                }

                _allTickets = displayItems;

                // Update summary counts from the full set
                TotalCount = _allTickets.Count;
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

        #endregion

        #region Search & Filter

        /// <summary>
        /// Applies search and filter criteria to <see cref="_allTickets"/> and updates
        /// <see cref="AssignedTickets"/>.
        ///
        /// Search rules (Req 28):
        ///   - Numeric input  → exact TicketID match
        ///   - Text input     → case-insensitive partial match on Title
        /// </summary>
        public void ApplyFilters()
        {
            var filtered = _allTickets.AsEnumerable();

            // --- Search by TicketID (exact) or Title (case-insensitive partial) ---
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim();

                if (int.TryParse(term, out int id))
                {
                    // Req 28.1: exact match on TicketID
                    filtered = filtered.Where(t => t.TicketID == id);
                }
                else
                {
                    // Req 28.2: case-insensitive partial match on Title
                    filtered = filtered.Where(t =>
                        t.Title.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
                }
            }

            // --- Status filter ---
            if (!string.IsNullOrEmpty(SelectedStatusFilter) && SelectedStatusFilter != "All")
                filtered = filtered.Where(t => t.Status == SelectedStatusFilter);

            // --- Priority filter ---
            if (!string.IsNullOrEmpty(SelectedPriorityFilter) && SelectedPriorityFilter != "All")
                filtered = filtered.Where(t => t.Priority == SelectedPriorityFilter);

            AssignedTickets = new ObservableCollection<AssignedTicketDisplayItem>(filtered);
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            SelectedStatusFilter = "All";
            SelectedPriorityFilter = "All";
            ApplyFilters();
        }

        #endregion
    }
}
