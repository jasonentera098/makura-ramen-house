# Design Document: IT Helpdesk Ticketing and Resolution System

## Overview

The IT Helpdesk Ticketing and Resolution System is a desktop WPF application built using C# and .NET 8.0, designed to manage IT support operations within an offline LAN environment. The system implements a three-tier architecture with clear separation between presentation (WPF Views), business logic (ViewModels and Services), and data access (Repository pattern with MS Access database).

### Key Architectural Decisions

1. **MVVM Pattern**: Implements Model-View-ViewModel architecture for clean separation of concerns and testability
2. **MS Access Database**: Uses OleDb provider for local/network database access without requiring SQL Server infrastructure
3. **Offline-First Design**: All operations work without internet connectivity, using LAN-based file shares for database and attachments
4. **Role-Based Security**: Three distinct user roles (Employee, Technician, Administrator) with different UI and data access permissions
5. **Repository Pattern**: Abstracts data access logic for maintainability and testability
6. **Service Layer**: Encapsulates business logic separate from UI and data access

### Technology Stack

- **Framework**: .NET 8.0 Windows Desktop (WPF)
- **Language**: C# 12
- **Database**: MS Access (.accdb) via System.Data.OleDb
- **UI Framework**: WPF with XAML
- **Architecture**: MVVM (Model-View-ViewModel)
- **Password Hashing**: System.Security.Cryptography (SHA256 or BCrypt)
- **Data Binding**: INotifyPropertyChanged, ObservableCollection
- **Dependency Injection**: Microsoft.Extensions.DependencyInjection (optional)

## Architecture

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                     Presentation Layer                       │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │   Views      │  │  ViewModels  │  │   Commands   │      │
│  │   (XAML)     │◄─┤  (Business   │◄─┤   (ICommand) │      │
│  │              │  │   Logic)     │  │              │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      Service Layer                           │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │ AuthService  │  │TicketService │  │  UserService │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│  ┌──────────────┐  ┌──────────────┐                         │
│  │ FileService  │  │ValidationSvc │                         │
│  └──────────────┘  └──────────────┘                         │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                   Data Access Layer                          │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐      │
│  │UserRepository│  │TicketRepo    │  │  DeptRepo    │      │
│  └──────────────┘  └──────────────┘  └──────────────┘      │
│  ┌──────────────┐                                            │
│  │DatabaseHelper│  (OleDbConnection, OleDbCommand)          │
│  └──────────────┘                                            │
└─────────────────────────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────┐
│                      Data Storage                            │
│  ┌──────────────┐  ┌──────────────┐                         │
│  │  MS Access   │  │  File System │                         │
│  │  Database    │  │ (Attachments)│                         │
│  │  (.accdb)    │  │              │                         │
│  └──────────────┘  └──────────────┘                         │
│  (Network Share or Local Path)                              │
└─────────────────────────────────────────────────────────────┘
```

### Layer Responsibilities

#### Presentation Layer
- **Views (XAML)**: Define UI structure, layout, and visual elements
- **ViewModels**: Handle UI logic, data binding, command execution, and state management
- **Commands**: Implement ICommand for button clicks and user actions

#### Service Layer
- **AuthenticationService**: Handles login, logout, password hashing, session management
- **TicketService**: Business logic for ticket creation, assignment, status updates
- **UserService**: User management operations (CRUD)
- **FileService**: Attachment upload, storage, retrieval
- **ValidationService**: Input validation, business rule enforcement

#### Data Access Layer
- **Repositories**: Abstract database operations with CRUD methods
- **DatabaseHelper**: Manages OleDb connections, executes queries, handles transactions
- **Models**: Plain C# classes representing database entities

### MVVM Implementation

```
View (XAML)
    ↕ (Data Binding)
ViewModel (INotifyPropertyChanged)
    ↕ (Method Calls)
Service Layer
    ↕ (Method Calls)
Repository Layer
    ↕ (SQL Queries)
Database
```

**Key MVVM Principles**:
1. Views have no code-behind logic (except InitializeComponent)
2. ViewModels expose properties and commands for binding
3. ViewModels never reference Views directly
4. Models are simple POCOs with no UI dependencies

## Components and Interfaces

### Core Models

#### User Model
```csharp
public class User
{
    public int UserID { get; set; }
    public string FullName { get; set; }
    public string Username { get; set; }
    public string PasswordHash { get; set; }
    public string Role { get; set; } // "Employee", "Technician", "Administrator"
    public int DepartmentID { get; set; }
    public string ContactNumber { get; set; }
    public DateTime CreatedAt { get; set; }
}
```

#### Ticket Model
```csharp
public class Ticket
{
    public int TicketID { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Category { get; set; } // Hardware, Software, Network, Access, Other
    public string Priority { get; set; } // High, Medium, Low
    public string Status { get; set; } // Pending, In Progress, Resolved, Closed
    public DateTime DateSubmitted { get; set; }
    public DateTime? DateResolved { get; set; }
    public int SubmittedBy { get; set; }
    public int? AssignedTo { get; set; }
    public int DepartmentID { get; set; }
}
```

#### TicketUpdate Model
```csharp
public class TicketUpdate
{
    public int UpdateID { get; set; }
    public int TicketID { get; set; }
    public int UpdatedBy { get; set; }
    public string Remarks { get; set; }
    public DateTime UpdateDate { get; set; }
    public string StatusChange { get; set; } // Optional: tracks status transitions
}
```

#### Attachment Model
```csharp
public class Attachment
{
    public int AttachmentID { get; set; }
    public int TicketID { get; set; }
    public string FilePath { get; set; }
    public string FileName { get; set; }
    public int UploadedBy { get; set; }
    public DateTime UploadedAt { get; set; }
}
```

#### Department Model
```csharp
public class Department
{
    public int DepartmentID { get; set; }
    public string DepartmentName { get; set; }
    public string Description { get; set; }
}
```

#### LoginLog Model
```csharp
public class LoginLog
{
    public int LogID { get; set; }
    public int UserID { get; set; }
    public DateTime LoginTime { get; set; }
    public DateTime? LogoutTime { get; set; }
}
```

### Service Interfaces

#### IAuthenticationService
```csharp
public interface IAuthenticationService
{
    Task<User> AuthenticateAsync(string username, string password);
    Task<bool> LogoutAsync(int userId);
    string HashPassword(string password);
    bool VerifyPassword(string password, string hash);
    User CurrentUser { get; }
}
```

#### ITicketService
```csharp
public interface ITicketService
{
    Task<int> CreateTicketAsync(Ticket ticket);
    Task<bool> UpdateTicketAsync(Ticket ticket);
    Task<bool> AssignTicketAsync(int ticketId, int technicianId, int assignedBy);
    Task<bool> UpdateStatusAsync(int ticketId, string newStatus, int updatedBy);
    Task<bool> AddRemarkAsync(int ticketId, string remarks, int updatedBy);
    Task<List<Ticket>> GetTicketsByUserAsync(int userId, string role);
    Task<List<Ticket>> SearchTicketsAsync(string searchTerm, Dictionary<string, object> filters);
    Task<Ticket> GetTicketByIdAsync(int ticketId);
}
```

#### IUserService
```csharp
public interface IUserService
{
    Task<int> CreateUserAsync(User user, string plainPassword);
    Task<bool> UpdateUserAsync(User user);
    Task<bool> DeleteUserAsync(int userId);
    Task<User> GetUserByIdAsync(int userId);
    Task<List<User>> GetAllUsersAsync();
    Task<List<User>> GetUsersByRoleAsync(string role);
    Task<bool> UsernameExistsAsync(string username);
}
```

#### IFileService
```csharp
public interface IFileService
{
    Task<string> SaveAttachmentAsync(string sourceFilePath, int ticketId);
    Task<bool> DeleteAttachmentAsync(string filePath);
    Task<bool> OpenAttachmentAsync(string filePath);
    Task<List<Attachment>> GetAttachmentsByTicketAsync(int ticketId);
}
```

### Repository Interfaces

#### IUserRepository
```csharp
public interface IUserRepository
{
    Task<User> GetByUsernameAsync(string username);
    Task<User> GetByIdAsync(int userId);
    Task<List<User>> GetAllAsync();
    Task<List<User>> GetByRoleAsync(string role);
    Task<int> InsertAsync(User user);
    Task<bool> UpdateAsync(User user);
    Task<bool> DeleteAsync(int userId);
    Task<bool> UsernameExistsAsync(string username);
}
```

#### ITicketRepository
```csharp
public interface ITicketRepository
{
    Task<int> InsertAsync(Ticket ticket);
    Task<bool> UpdateAsync(Ticket ticket);
    Task<Ticket> GetByIdAsync(int ticketId);
    Task<List<Ticket>> GetBySubmitterAsync(int userId);
    Task<List<Ticket>> GetByAssigneeAsync(int userId);
    Task<List<Ticket>> GetByStatusAsync(string status);
    Task<List<Ticket>> SearchAsync(string searchTerm, Dictionary<string, object> filters);
    Task<List<Ticket>> GetAllAsync();
}
```


### ViewModels

#### Base ViewModel
```csharp
public abstract class ViewModelBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler PropertyChanged;
    
    protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
    
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
            return false;
        
        field = value;
        OnPropertyChanged(propertyName);
        return true;
    }
}
```

#### LoginViewModel
```csharp
public class LoginViewModel : ViewModelBase
{
    private readonly IAuthenticationService _authService;
    private string _username;
    private string _password;
    private string _errorMessage;
    private bool _isLoading;
    
    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }
    
    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }
    
    public string ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }
    
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }
    
    public ICommand LoginCommand { get; }
    
    public LoginViewModel(IAuthenticationService authService)
    {
        _authService = authService;
        LoginCommand = new RelayCommand(async () => await LoginAsync(), CanLogin);
    }
    
    private bool CanLogin() => !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
    
    private async Task LoginAsync()
    {
        IsLoading = true;
        ErrorMessage = string.Empty;
        
        try
        {
            var user = await _authService.AuthenticateAsync(Username, Password);
            if (user != null)
            {
                // Navigate to appropriate dashboard based on role
                NavigateToDashboard(user.Role);
            }
            else
            {
                ErrorMessage = "Invalid username or password";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Login failed: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}
```

#### EmployeeDashboardViewModel
```csharp
public class EmployeeDashboardViewModel : ViewModelBase
{
    private readonly ITicketService _ticketService;
    private readonly IAuthenticationService _authService;
    private ObservableCollection<Ticket> _myTickets;
    private int _pendingCount;
    private int _inProgressCount;
    private int _resolvedCount;
    private Ticket _selectedTicket;
    
    public ObservableCollection<Ticket> MyTickets
    {
        get => _myTickets;
        set => SetProperty(ref _myTickets, value);
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
    
    public Ticket SelectedTicket
    {
        get => _selectedTicket;
        set => SetProperty(ref _selectedTicket, value);
    }
    
    public ICommand SubmitTicketCommand { get; }
    public ICommand ViewTicketCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand LogoutCommand { get; }
    
    public EmployeeDashboardViewModel(ITicketService ticketService, IAuthenticationService authService)
    {
        _ticketService = ticketService;
        _authService = authService;
        
        SubmitTicketCommand = new RelayCommand(NavigateToSubmitTicket);
        ViewTicketCommand = new RelayCommand<Ticket>(ViewTicketDetails);
        RefreshCommand = new RelayCommand(async () => await LoadTicketsAsync());
        LogoutCommand = new RelayCommand(Logout);
        
        LoadTicketsAsync();
    }
    
    private async Task LoadTicketsAsync()
    {
        var tickets = await _ticketService.GetTicketsByUserAsync(_authService.CurrentUser.UserID, "Employee");
        MyTickets = new ObservableCollection<Ticket>(tickets);
        
        PendingCount = tickets.Count(t => t.Status == "Pending");
        InProgressCount = tickets.Count(t => t.Status == "In Progress");
        ResolvedCount = tickets.Count(t => t.Status == "Resolved" || t.Status == "Closed");
    }
}
```

#### TechnicianDashboardViewModel
```csharp
public class TechnicianDashboardViewModel : ViewModelBase
{
    private readonly ITicketService _ticketService;
    private readonly IAuthenticationService _authService;
    private ObservableCollection<Ticket> _assignedTickets;
    private int _pendingCount;
    private int _inProgressCount;
    private int _resolvedCount;
    private int _totalCount;
    
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
    
    public ICommand UpdateTicketCommand { get; }
    public ICommand ViewTicketCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand LogoutCommand { get; }
    
    public TechnicianDashboardViewModel(ITicketService ticketService, IAuthenticationService authService)
    {
        _ticketService = ticketService;
        _authService = authService;
        
        UpdateTicketCommand = new RelayCommand<Ticket>(NavigateToUpdateTicket);
        ViewTicketCommand = new RelayCommand<Ticket>(ViewTicketDetails);
        RefreshCommand = new RelayCommand(async () => await LoadTicketsAsync());
        LogoutCommand = new RelayCommand(Logout);
        
        LoadTicketsAsync();
    }
    
    private async Task LoadTicketsAsync()
    {
        var tickets = await _ticketService.GetTicketsByUserAsync(_authService.CurrentUser.UserID, "Technician");
        AssignedTickets = new ObservableCollection<Ticket>(tickets);
        
        PendingCount = tickets.Count(t => t.Status == "Pending");
        InProgressCount = tickets.Count(t => t.Status == "In Progress");
        ResolvedCount = tickets.Count(t => t.Status == "Resolved");
        TotalCount = tickets.Count;
    }
}
```

#### AdminDashboardViewModel
```csharp
public class AdminDashboardViewModel : ViewModelBase
{
    private readonly ITicketService _ticketService;
    private readonly IUserService _userService;
    private ObservableCollection<Ticket> _allTickets;
    private ObservableCollection<User> _allUsers;
    private int _totalTickets;
    private int _pendingTickets;
    private int _resolvedTickets;
    
    public ObservableCollection<Ticket> AllTickets
    {
        get => _allTickets;
        set => SetProperty(ref _allTickets, value);
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
    
    public ICommand AssignTicketCommand { get; }
    public ICommand ManageUsersCommand { get; }
    public ICommand GenerateReportCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand LogoutCommand { get; }
    
    public AdminDashboardViewModel(ITicketService ticketService, IUserService userService)
    {
        _ticketService = ticketService;
        _userService = userService;
        
        AssignTicketCommand = new RelayCommand<Ticket>(NavigateToAssignTicket);
        ManageUsersCommand = new RelayCommand(NavigateToUserManagement);
        GenerateReportCommand = new RelayCommand(NavigateToReports);
        RefreshCommand = new RelayCommand(async () => await LoadDataAsync());
        LogoutCommand = new RelayCommand(Logout);
        
        LoadDataAsync();
    }
    
    private async Task LoadDataAsync()
    {
        var tickets = await _ticketService.GetTicketsByUserAsync(0, "Administrator");
        AllTickets = new ObservableCollection<Ticket>(tickets);
        
        var users = await _userService.GetAllUsersAsync();
        AllUsers = new ObservableCollection<User>(users);
        
        TotalTickets = tickets.Count;
        PendingTickets = tickets.Count(t => t.Status == "Pending");
        ResolvedTickets = tickets.Count(t => t.Status == "Resolved" || t.Status == "Closed");
    }
}
```

### UI Navigation Flow

```
MainWindow (Login)
    │
    ├─► EmployeeDashboard (Role: Employee)
    │       ├─► TicketSubmitPage
    │       ├─► MyTicketsView
    │       └─► TicketDetailsView
    │
    ├─► TechnicianDashboard (Role: Technician)
    │       ├─► AssignedTicketsView
    │       ├─► UpdateTicketPage
    │       ├─► TicketDetailsView
    │       └─► Reports
    │
    └─► AdminDashboard (Role: Administrator)
            ├─► AllTicketsView
            ├─► AssignTicketPage
            ├─► UserManagementView
            │       ├─► AddUser
            │       ├─► EditUser
            │       └─► DeleteConfirmation
            ├─► DepartmentManagementView
            ├─► Reports
            └─► LoginLogsView
```

## Data Models

### Database Schema

#### Users Table
```sql
CREATE TABLE Users (
    UserID AUTOINCREMENT PRIMARY KEY,
    FullName TEXT(100) NOT NULL,
    Username TEXT(50) NOT NULL UNIQUE,
    PasswordHash TEXT(255) NOT NULL,
    Role TEXT(20) NOT NULL CHECK (Role IN ('Employee', 'Technician', 'Administrator')),
    DepartmentID INTEGER NOT NULL,
    ContactNumber TEXT(20),
    CreatedAt DATETIME NOT NULL DEFAULT NOW(),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);
```

#### Departments Table
```sql
CREATE TABLE Departments (
    DepartmentID AUTOINCREMENT PRIMARY KEY,
    DepartmentName TEXT(100) NOT NULL UNIQUE,
    Description TEXT(255)
);
```

#### Tickets Table
```sql
CREATE TABLE Tickets (
    TicketID AUTOINCREMENT PRIMARY KEY,
    Title TEXT(200) NOT NULL,
    Description MEMO NOT NULL,
    Category TEXT(50) NOT NULL CHECK (Category IN ('Hardware', 'Software', 'Network', 'Access', 'Other')),
    Priority TEXT(20) NOT NULL CHECK (Priority IN ('High', 'Medium', 'Low')),
    Status TEXT(20) NOT NULL CHECK (Status IN ('Pending', 'In Progress', 'Resolved', 'Closed')),
    DateSubmitted DATETIME NOT NULL DEFAULT NOW(),
    DateResolved DATETIME,
    SubmittedBy INTEGER NOT NULL,
    AssignedTo INTEGER,
    DepartmentID INTEGER NOT NULL,
    FOREIGN KEY (SubmittedBy) REFERENCES Users(UserID),
    FOREIGN KEY (AssignedTo) REFERENCES Users(UserID),
    FOREIGN KEY (DepartmentID) REFERENCES Departments(DepartmentID)
);
```

#### TicketUpdates Table
```sql
CREATE TABLE TicketUpdates (
    UpdateID AUTOINCREMENT PRIMARY KEY,
    TicketID INTEGER NOT NULL,
    UpdatedBy INTEGER NOT NULL,
    Remarks MEMO NOT NULL,
    UpdateDate DATETIME NOT NULL DEFAULT NOW(),
    StatusChange TEXT(50),
    FOREIGN KEY (TicketID) REFERENCES Tickets(TicketID),
    FOREIGN KEY (UpdatedBy) REFERENCES Users(UserID)
);
```

#### Attachments Table
```sql
CREATE TABLE Attachments (
    AttachmentID AUTOINCREMENT PRIMARY KEY,
    TicketID INTEGER NOT NULL,
    FilePath TEXT(500) NOT NULL,
    FileName TEXT(255) NOT NULL,
    UploadedBy INTEGER NOT NULL,
    UploadedAt DATETIME NOT NULL DEFAULT NOW(),
    FOREIGN KEY (TicketID) REFERENCES Tickets(TicketID),
    FOREIGN KEY (UploadedBy) REFERENCES Users(UserID)
);
```

#### LoginLogs Table
```sql
CREATE TABLE LoginLogs (
    LogID AUTOINCREMENT PRIMARY KEY,
    UserID INTEGER NOT NULL,
    LoginTime DATETIME NOT NULL DEFAULT NOW(),
    LogoutTime DATETIME,
    FOREIGN KEY (UserID) REFERENCES Users(UserID)
);
```

### Entity Relationships

```
Departments (1) ──────< (N) Users
                              │
                              │ SubmittedBy
                              ▼
                         (N) Tickets (N)
                              │         │
                              │         │ AssignedTo
                              │         ▼
                              │    (1) Users (Technician)
                              │
                              ├──< (N) TicketUpdates
                              │
                              └──< (N) Attachments

Users (1) ──────< (N) LoginLogs
```

### Data Access Patterns

#### Connection String Management
```csharp
public class DatabaseConfig
{
    public static string ConnectionString => 
        $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={DatabasePath};Persist Security Info=False;";
    
    public static string DatabasePath => 
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HelpdeskDB.accdb");
    
    // For network share:
    // public static string DatabasePath => @"\\ServerName\Share\HelpdeskDB.accdb";
}
```

#### Repository Base Pattern
```csharp
public abstract class RepositoryBase
{
    protected readonly string _connectionString;
    
    protected RepositoryBase(string connectionString)
    {
        _connectionString = connectionString;
    }
    
    protected async Task<T> ExecuteScalarAsync<T>(string query, OleDbParameter[] parameters = null)
    {
        using (var connection = new OleDbConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new OleDbCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);
                
                var result = await command.ExecuteScalarAsync();
                return (T)Convert.ChangeType(result, typeof(T));
            }
        }
    }
    
    protected async Task<int> ExecuteNonQueryAsync(string query, OleDbParameter[] parameters = null)
    {
        using (var connection = new OleDbConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new OleDbCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);
                
                return await command.ExecuteNonQueryAsync();
            }
        }
    }
    
    protected async Task<List<T>> ExecuteReaderAsync<T>(string query, Func<OleDbDataReader, T> mapper, OleDbParameter[] parameters = null)
    {
        var results = new List<T>();
        using (var connection = new OleDbConnection(_connectionString))
        {
            await connection.OpenAsync();
            using (var command = new OleDbCommand(query, connection))
            {
                if (parameters != null)
                    command.Parameters.AddRange(parameters);
                
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        results.Add(mapper(reader));
                    }
                }
            }
        }
        return results;
    }
}
```


## Security Design

### Password Hashing
```csharp
public class PasswordHasher
{
    public static string HashPassword(string password)
    {
        using (var sha256 = SHA256.Create())
        {
            var saltedPassword = $"ITHelpdesk_{password}_Salt2024";
            var bytes = Encoding.UTF8.GetBytes(saltedPassword);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }

    public static bool VerifyPassword(string password, string storedHash)
    {
        var hash = HashPassword(password);
        return hash == storedHash;
    }
}
```

### Session Management
```csharp
public static class SessionManager
{
    public static User CurrentUser { get; private set; }
    public static int CurrentLogID { get; private set; }

    public static void StartSession(User user, int logId)
    {
        CurrentUser = user;
        CurrentLogID = logId;
    }

    public static void EndSession()
    {
        CurrentUser = null;
        CurrentLogID = 0;
    }

    public static bool IsAuthenticated => CurrentUser != null;
    public static string CurrentRole => CurrentUser?.Role;
}
```

### Role-Based Access Control
- **Employee**: Can only access EmployeeDashboard, TicketSubmitPage, MyTickets, and Employee Reports
- **Technician**: Can only access TechnicianDashboard, AssignedTickets, UpdateTicket, and Technician Reports
- **Administrator**: Full access to all windows and features

## File Storage Design

### Attachment Storage Strategy
```
Application Root
└── Attachments/
    ├── Ticket_1/
    │   ├── screenshot_001.png
    │   └── error_log.txt
    ├── Ticket_2/
    │   └── network_diagram.pdf
    └── Ticket_N/
        └── ...
```

### File Service Implementation
```csharp
public class FileService : IFileService
{
    private readonly string _baseStoragePath;

    public FileService()
    {
        _baseStoragePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Attachments");
        Directory.CreateDirectory(_baseStoragePath);
    }

    public async Task<string> SaveAttachmentAsync(string sourceFilePath, int ticketId)
    {
        var ticketFolder = Path.Combine(_baseStoragePath, $"Ticket_{ticketId}");
        Directory.CreateDirectory(ticketFolder);

        var fileName = Path.GetFileName(sourceFilePath);
        var destPath = Path.Combine(ticketFolder, fileName);

        // Handle duplicate filenames
        if (File.Exists(destPath))
        {
            var nameWithoutExt = Path.GetFileNameWithoutExtension(fileName);
            var ext = Path.GetExtension(fileName);
            destPath = Path.Combine(ticketFolder, $"{nameWithoutExt}_{DateTime.Now:yyyyMMddHHmmss}{ext}");
        }

        await Task.Run(() => File.Copy(sourceFilePath, destPath));
        return destPath;
    }

    public async Task<bool> OpenAttachmentAsync(string filePath)
    {
        if (!File.Exists(filePath))
            return false;

        await Task.Run(() => Process.Start(new ProcessStartInfo(filePath) { UseShellExecute = true }));
        return true;
    }
}
```

## UI Design Specifications

### Window Inventory

| Window/Page | Role Access | Purpose |
|---|---|---|
| MainWindow.xaml | All | Login screen |
| Window1.xaml | Admin | Admin main dashboard |
| Window2.xaml | Admin | Ticket management / User management |
| EmployeeDashboard.xaml | Employee | Employee main dashboard |
| TicketSubmitPage.xaml | Employee, Technician | Submit/Update ticket |
| Reports.xaml | Employee | Employee ticket summary report |
| Reportss.xaml | Admin, Technician | Full system reports |
| AddUser.xaml | Admin | Add new user form |
| DeleteConfirmation.xaml | Admin | Delete confirmation dialog |

### Color Scheme and Styling
- **Primary Color**: `#007ACC` (Blue - IT/Tech theme)
- **Secondary Color**: `#F0F0F0` (Light Gray - backgrounds)
- **Success**: `#28A745` (Green - resolved/success)
- **Warning**: `#FFC107` (Yellow/Orange - pending/in progress)
- **Danger**: `#DC3545` (Red - high priority/delete)
- **Text**: `#333333` (Dark Gray)

### Priority Color Coding
- **High**: Red badge `#DC3545`
- **Medium**: Orange badge `#FD7E14`
- **Low**: Green badge `#28A745`

### Status Color Coding
- **Pending**: Gray `#6C757D`
- **In Progress**: Blue `#007ACC`
- **Resolved**: Green `#28A745`
- **Closed**: Dark Gray `#343A40`

## Error Handling Strategy

### Global Exception Handler
```csharp
public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        
        DispatcherUnhandledException += (s, ex) =>
        {
            MessageBox.Show($"An unexpected error occurred: {ex.Exception.Message}", 
                "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            ex.Handled = true;
        };
    }
}
```

### Database Error Handling
- All database operations wrapped in try-catch blocks
- User-friendly error messages displayed via MessageBox
- Connection failures show "Unable to connect to the database. Please check your network connection."
- Parameterized queries used throughout to prevent SQL injection

## LAN Configuration

### Database Path Configuration
The application supports two deployment modes:
1. **Local Mode**: Database stored in application directory
2. **LAN Mode**: Database stored on a network share

```csharp
public class AppSettings
{
    private static readonly string ConfigPath = 
        Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "appsettings.ini");

    public static string DatabasePath
    {
        get
        {
            // Read from config file, fallback to local
            if (File.Exists(ConfigPath))
            {
                var lines = File.ReadAllLines(ConfigPath);
                var dbLine = lines.FirstOrDefault(l => l.StartsWith("DatabasePath="));
                if (dbLine != null)
                    return dbLine.Split('=')[1].Trim();
            }
            return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "HelpdeskDB.accdb");
        }
    }
}
```

### Network Share Setup
For LAN deployment, the MS Access `.accdb` file is placed on a shared network folder:
- Example path: `\\OFFICE-SERVER\ITHelpdesk\HelpdeskDB.accdb`
- All client machines connect to the same database file
- Attachments folder also shared: `\\OFFICE-SERVER\ITHelpdesk\Attachments\`

## Correctness Properties

### Property 1: Ticket Lifecycle Integrity
Every ticket must follow the valid state progression: `Pending → In Progress → Resolved → Closed`. No ticket shall skip states or move backwards.

### Property 2: Role Access Isolation
A user with role X shall never be able to access features designated for role Y. Employee cannot assign tickets; Technician cannot manage users; only Admin can close tickets.

### Property 3: Authentication Completeness
Every active session must have a corresponding LoginLog entry with a LoginTime. Every completed session must have a LogoutTime.

### Property 4: Referential Integrity
Every Ticket must reference a valid SubmittedBy UserID and DepartmentID. Every TicketUpdate must reference a valid TicketID and UpdatedBy UserID. Every Attachment must reference a valid TicketID.

### Property 5: Password Security
No plaintext password shall ever be stored in the database. All stored passwords must be SHA256 hashes.

### Property 6: Offline Operation
The system shall never attempt any network call outside the local LAN. No HTTP/HTTPS requests, no DNS lookups to external domains, no cloud service calls.
