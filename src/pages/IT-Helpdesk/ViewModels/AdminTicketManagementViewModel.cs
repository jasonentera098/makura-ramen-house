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
    public class TicketDisplayItem
    {
        public int TicketID { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Priority { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string AssignedToName { get; set; } = "Unassigned";
    }

    public class AdminTicketManagementViewModel : ViewModelBase
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
        private int _closedCount;
        private string _errorMessage = string.Empty;

        public AdminTicketManagementViewModel()
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
            UpdateTicketCommand = new RelayCommand(
                () => UpdateTicket(),
                () => SelectedTicket != null);
            AssignTicketCommand = new RelayCommand(
                async () => await AssignTicketAsync(),
                () => SelectedTicket != null && SelectedTicket.AssignedToName == "Unassigned");
            // CanExecute allows clicking when any ticket is selected; CloseTicketAsync validates status inline
            CloseTicketCommand = new RelayCommand(
                async () => await CloseTicketAsync(),
                () => SelectedTicket != null);

            _ = LoadTicketsAsync();
        }

        public AdminTicketManagementViewModel(ITicketService ticketService, IUserService userService)
        {
            _ticketService = ticketService ?? throw new ArgumentNullException(nameof(ticketService));
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));

            SearchCommand = new RelayCommand(() => ApplyFilters());
            ClearSearchCommand = new RelayCommand(ClearSearch);
            RefreshCommand = new RelayCommand(async () => await LoadTicketsAsync());

            ViewTicketCommand = new RelayCommand(
                () => ViewTicketDetails(),
                () => SelectedTicket != null);
            UpdateTicketCommand = new RelayCommand(
                () => UpdateTicket(),
                () => SelectedTicket != null);
            AssignTicketCommand = new RelayCommand(
                async () => await AssignTicketAsync(),
                () => SelectedTicket != null && SelectedTicket.AssignedToName == "Unassigned");
            // CanExecute allows clicking when any ticket is selected; CloseTicketAsync validates status inline
            CloseTicketCommand = new RelayCommand(
                async () => await CloseTicketAsync(),
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
                // Clear any action-level error when the user selects a different ticket (Requirement 20.6)
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

        public int ClosedCount
        {
            get => _closedCount;
            set => SetProperty(ref _closedCount, value);
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; }
        public ICommand ClearSearchCommand { get; }
        public ICommand RefreshCommand { get; }
        public ICommand ViewTicketCommand { get; }
        public ICommand UpdateTicketCommand { get; }
        public ICommand AssignTicketCommand { get; }
        public ICommand CloseTicketCommand { get; }

        #endregion

        #region Data Loading

        public async Task LoadTicketsAsync()
        {
            try
            {
                IsLoading = true;

                var tickets = await _ticketService.GetTicketsByUserAsync(0, "Administrator");

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
                ClosedCount = _allTickets.Count(t => t.Status == "Closed");

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

        private async Task CloseTicketAsync()
        {
            if (SelectedTicket == null) return;

            if (SelectedTicket.Status != "Resolved")
            {
                ErrorMessage = $"Ticket #{SelectedTicket.TicketID} cannot be closed. Only tickets with 'Resolved' status can be closed.";
                return;
            }

            ErrorMessage = string.Empty;

            var confirm = MessageBox.Show(
                $"Are you sure you want to close ticket #{SelectedTicket.TicketID}: \"{SelectedTicket.Title}\"?",
                "Confirm Close", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                int adminId = SessionManager.CurrentUser!.UserID;
                bool success = await _ticketService.UpdateStatusAsync(SelectedTicket.TicketID, "Closed", adminId);

                if (success)
                {
                    MessageBox.Show($"Ticket #{SelectedTicket.TicketID} has been closed successfully.",
                        "Ticket Closed", MessageBoxButton.OK, MessageBoxImage.Information);
                    await LoadTicketsAsync();
                }
                else
                {
                    ErrorMessage = $"Failed to close ticket #{SelectedTicket.TicketID}. Please try again.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error closing ticket: {ex.Message}";
            }
        }

        private async Task AssignTicketAsync()
        {
            if (SelectedTicket == null) 
            {
                MessageBox.Show("Please select a ticket first.", "No Ticket Selected", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            ErrorMessage = string.Empty;

            try
            {
                var technicians = await _userService.GetActiveUsersByRoleAsync("Technician");
                if (technicians == null || technicians.Count == 0)
                {
                    ErrorMessage = "No active technicians are available to assign. Please add a technician account first.";
                    MessageBox.Show(ErrorMessage, "No Technicians Available", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }

                // Build workload information for each technician
                var technicianWorkloads = new List<TechnicianWorkloadInfo>();
                
                foreach (var tech in technicians)
                {
                    // Get all tickets assigned to this technician
                    var assignedTickets = await _ticketService.GetTicketsByUserAsync(tech.UserID, "Technician");
                    
                    // Count only active tickets (not Closed or Resolved)
                    int activeTicketCount = assignedTickets.Count(t => 
                        t.Status != "Closed" && t.Status != "Resolved");
                    
                    // Determine status based on workload
                    string status;
                    int workloadPercentage;
                    
                    if (tech.AccountStatus == "Inactive")
                    {
                        status = "On Leave";
                        workloadPercentage = 0;
                    }
                    else if (activeTicketCount == 0)
                    {
                        status = "Available";
                        workloadPercentage = 0;
                    }
                    else if (activeTicketCount <= 5)
                    {
                        status = "Available";
                        workloadPercentage = (activeTicketCount * 100) / 15; // 5 tickets = 33%
                    }
                    else if (activeTicketCount <= 10)
                    {
                        status = "Busy";
                        workloadPercentage = (activeTicketCount * 100) / 15; // 10 tickets = 67%
                    }
                    else
                    {
                        status = "Overloaded";
                        workloadPercentage = Math.Min(100, (activeTicketCount * 100) / 15); // Cap at 100%
                    }
                    
                    // Calculate bar width (max 200px for 100%)
                    double barWidth = (workloadPercentage / 100.0) * 200.0;
                    
                    technicianWorkloads.Add(new TechnicianWorkloadInfo
                    {
                        UserID = tech.UserID,
                        FullName = tech.FullName,
                        Username = tech.Username,
                        Status = status,
                        AssignedTicketsCount = activeTicketCount,
                        WorkloadPercentage = workloadPercentage,
                        WorkloadBarWidth = barWidth,
                        AccountStatus = tech.AccountStatus
                    });
                }
                
                // Sort by workload (Available first, then by ticket count)
                technicianWorkloads = technicianWorkloads
                    .OrderBy(t => t.Status == "On Leave" ? 3 : (t.Status == "Overloaded" ? 2 : (t.Status == "Busy" ? 1 : 0)))
                    .ThenBy(t => t.AssignedTicketsCount)
                    .ToList();

                var dialog = new AssignTicketDialog(SelectedTicket.TicketID, SelectedTicket.Title, technicianWorkloads);
                var owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive)
                            ?? Application.Current.Windows.OfType<Window1>().FirstOrDefault();

                if (owner != null)
                    dialog.Owner = owner;

                bool? dialogResult = dialog.ShowDialog();

                if (dialogResult == true)
                {
                    int technicianId = dialog.SelectedTechnicianId;
                    int assignedBy = SessionManager.CurrentUser!.UserID;

                    bool success = await _ticketService.AssignTicketAsync(SelectedTicket.TicketID, technicianId, assignedBy);
                    if (success)
                    {
                        MessageBox.Show($"Ticket #{SelectedTicket.TicketID} has been assigned successfully.",
                            "Assignment Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                        await LoadTicketsAsync();
                    }
                    else
                    {
                        ErrorMessage = $"Failed to assign ticket #{SelectedTicket.TicketID}. Please try again.";
                        MessageBox.Show(ErrorMessage, "Assignment Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Error assigning ticket: {ex.Message}";
                MessageBox.Show($"Exception occurred: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
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

        private void UpdateTicket()
        {
            if (SelectedTicket == null) return;

            var dialog = new AdminTicketUpdateDialog(SelectedTicket.TicketID);
            var owner = Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            if (owner != null)
                dialog.Owner = owner;
            
            bool? result = dialog.ShowDialog();
            
            // Refresh the list if the dialog was closed with success
            if (result == true)
            {
                _ = LoadTicketsAsync();
            }
        }

        #endregion
    }
}
