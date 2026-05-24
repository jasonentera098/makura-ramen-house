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
    public class AdminUserManagementViewModel : ViewModelBase
    {
        private readonly IUserService _userService;

        private List<User> _allUsers = new();
        private ObservableCollection<User> _filteredUsers = new();
        private User? _selectedUser;
        private string _searchText = string.Empty;
        private string _selectedRoleFilter = "All";
        private bool _isLoading;
        private int _totalCount;
        private int _employeeCount;
        private int _technicianCount;
        private int _adminCount;

        public AdminUserManagementViewModel()
        {
            var connStr = DatabaseConfig.ConnectionString;
            var userRepo = new UserRepository(connStr);
            var ticketRepo = new TicketRepository(connStr);
            _userService = new UserService(userRepo, ticketRepo);

            InitializeCommands();
            _ = LoadUsersAsync();
        }

        public AdminUserManagementViewModel(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
            InitializeCommands();
            _ = LoadUsersAsync();
        }

        private void InitializeCommands()
        {
            SearchCommand = new RelayCommand(() => ApplyFilters());
            ClearSearchCommand = new RelayCommand(ClearSearch);
            RefreshCommand = new RelayCommand(async () => await LoadUsersAsync());
            AddUserCommand = new RelayCommand(OnAddUser);
            EditUserCommand = new RelayCommand(OnEditUser, () => SelectedUser != null);
            // Delete is only allowed for Employee and Technician roles — not Administrator
            DeleteUserCommand = new RelayCommand(OnDeleteUser, () =>
                SelectedUser != null && SelectedUser.Role != "Administrator");
        }

        #region Properties

        public ObservableCollection<User> FilteredUsers
        {
            get => _filteredUsers;
            set => SetProperty(ref _filteredUsers, value);
        }

        public User? SelectedUser
        {
            get => _selectedUser;
            set
            {
                SetProperty(ref _selectedUser, value);
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        public string SelectedRoleFilter
        {
            get => _selectedRoleFilter;
            set => SetProperty(ref _selectedRoleFilter, value);
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

        public int EmployeeCount
        {
            get => _employeeCount;
            set => SetProperty(ref _employeeCount, value);
        }

        public int TechnicianCount
        {
            get => _technicianCount;
            set => SetProperty(ref _technicianCount, value);
        }

        public int AdminCount
        {
            get => _adminCount;
            set => SetProperty(ref _adminCount, value);
        }

        #endregion

        #region Commands

        public ICommand SearchCommand { get; private set; } = null!;
        public ICommand ClearSearchCommand { get; private set; } = null!;
        public ICommand RefreshCommand { get; private set; } = null!;
        public ICommand AddUserCommand { get; private set; } = null!;
        public ICommand EditUserCommand { get; private set; } = null!;
        public ICommand DeleteUserCommand { get; private set; } = null!;

        #endregion

        #region Data Loading

        public async Task LoadUsersAsync()
        {
            try
            {
                IsLoading = true;
                var users = await _userService.GetAllUsersAsync();
                _allUsers = users;

                TotalCount = users.Count;
                EmployeeCount = users.Count(u => u.Role == "Employee");
                TechnicianCount = users.Count(u => u.Role == "Technician");
                AdminCount = users.Count(u => u.Role == "Administrator");

                ApplyFilters();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading users: {ex.Message}",
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }

        public void ApplyFilters()
        {
            var filtered = _allUsers.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                var term = SearchText.Trim();
                filtered = filtered.Where(u =>
                    u.FullName.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    u.Username.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    u.ContactNumber.IndexOf(term, StringComparison.OrdinalIgnoreCase) >= 0);
            }

            if (!string.IsNullOrEmpty(SelectedRoleFilter) && SelectedRoleFilter != "All")
                filtered = filtered.Where(u => u.Role == SelectedRoleFilter);

            FilteredUsers = new ObservableCollection<User>(filtered);
        }

        private void ClearSearch()
        {
            SearchText = string.Empty;
            SelectedRoleFilter = "All";
            ApplyFilters();
        }

        #endregion

        #region Command Handlers

        private void OnAddUser()
        {
            try
            {
                var parentWindow = Application.Current.Windows.OfType<Window1>().FirstOrDefault();

                // Use WindowManager to prevent multiple AddUser (add-mode) instances.
                // Because AddUser can also be opened in edit mode (with a User argument), we
                // only guard the add-mode instance here; edit windows are distinct per-user.
                bool? result = WindowManager.ShowSingleDialog<AddUser>(
                    () => new AddUser(),
                    parentWindow);

                if (result == true)
                {
                    _ = LoadUsersAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Add User window: {ex.Message}", 
                    "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void OnEditUser()
        {
            if (SelectedUser == null) return;

            var parentWindow = Application.Current.Windows.OfType<Window1>().FirstOrDefault();

            // For edit mode, check if an AddUser window is already open (any instance).
            // If one is open, activate it instead of opening another.
            var existingAddUser = Application.Current.Windows.OfType<AddUser>().FirstOrDefault();
            if (existingAddUser != null)
            {
                existingAddUser.Activate();
                return;
            }

            var editWindow = new AddUser(SelectedUser);
            if (parentWindow != null)
                editWindow.Owner = parentWindow;

            bool? result = editWindow.ShowDialog();

            // Return focus to the parent window after the dialog closes (Requirement 18.3)
            parentWindow?.Activate();

            if (result == true)
                _ = LoadUsersAsync();
        }

        private void OnDeleteUser()
        {
            if (SelectedUser == null) return;

            var parentWindow = Application.Current.Windows.OfType<Window1>().FirstOrDefault();

            // Use WindowManager to prevent multiple DeleteConfirmation dialogs.
            bool? result = WindowManager.ShowSingleDialog<DeleteConfirmation>(
                () => new DeleteConfirmation(SelectedUser),
                parentWindow);

            if (result == true)
                _ = LoadUsersAsync();
        }

        #endregion
    }
}
