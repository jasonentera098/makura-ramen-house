# Implementation Tasks: IT Helpdesk Ticketing and Resolution System

## Overview
Implementation plan for the WPF C# desktop application with MS Access database, operating offline on LAN only.

---

## Task 1: Project Setup and Database Foundation

- [x] 1.1 Configure the .csproj to include required NuGet packages (System.Data.OleDb, Microsoft.Extensions.DependencyInjection)
- [x] 1.2 Create the MS Access database file (`HelpdeskDB.accdb`) with all 6 tables: Users, Departments, Tickets, TicketUpdates, Attachments, LoginLogs
- [x] 1.3 Seed the database with default departments (Human Resources, Accounting, Sales, Operations, Registrar, IT Department)
- [x] 1.4 Seed a default Administrator account (username: `admin`, password: hashed `admin123`)
- [x] 1.5 Create the `DatabaseHelper` class with OleDb connection management and parameterized query support
- [x] 1.6 Create the `AppSettings` class for configurable database path (local and LAN UNC path support)
- [x] 1.7 Create the `Attachments/` folder structure in the application directory

---

## Task 2: Core Models and Base Classes

- [x] 2.1 Create model classes: `User`, `Ticket`, `TicketUpdate`, `Attachment`, `Department`, `LoginLog`
- [x] 2.2 Create `ViewModelBase` with `INotifyPropertyChanged` and `SetProperty` helper
- [x] 2.3 Create `RelayCommand` implementing `ICommand` for ViewModel commands
- [x] 2.4 Create `SessionManager` static class for current user session tracking
- [x] 2.5 Create `PasswordHasher` utility class using SHA256

---

## Task 3: Data Access Layer (Repositories)

- [x] 3.1 Create `RepositoryBase` with async `ExecuteScalar`, `ExecuteNonQuery`, and `ExecuteReader` helpers
- [x] 3.2 Implement `UserRepository` (GetByUsername, GetById, GetAll, GetByRole, Insert, Update, Delete, UsernameExists)
- [x] 3.3 Implement `DepartmentRepository` (GetAll, GetById, Insert, Update, Delete, HasDependencies)
- [x] 3.4 Implement `TicketRepository` (Insert, Update, GetById, GetBySubmitter, GetByAssignee, GetAll, Search with filters)
- [x] 3.5 Implement `TicketUpdateRepository` (Insert, GetByTicketId ordered by date)
- [x] 3.6 Implement `AttachmentRepository` (Insert, GetByTicketId, Delete)
- [x] 3.7 Implement `LoginLogRepository` (Insert login, UpdateLogout, GetByUserId, GetAll with date filter)

---

## Task 4: Service Layer

- [x] 4.1 Implement `AuthenticationService` (Authenticate, Logout, HashPassword, VerifyPassword, CurrentUser)
- [x] 4.2 Implement `TicketService` (CreateTicket, UpdateTicket, AssignTicket, UpdateStatus, AddRemark, GetTickets, Search)
- [x] 4.3 Implement `UserService` (CreateUser, UpdateUser, DeleteUser, GetUsers, UsernameExists)
- [x] 4.4 Implement `DepartmentService` (CreateDepartment, UpdateDepartment, DeleteDepartment, GetDepartments)
- [x] 4.5 Implement `FileService` (SaveAttachment, DeleteAttachment, OpenAttachment, GetAttachmentsByTicket)
- [x] 4.6 Implement `ReportService` (GetTicketSummary, GetTicketsByFilters, GetTechnicianPerformance)

---

## Task 5: Login Window (MainWindow.xaml)

- [x] 5.1 Design the Login UI with logo, title "IT Helpdesk System", Username TextBox, Password PasswordBox, and LOG IN button
- [x] 5.2 Create `LoginViewModel` with Username, Password, ErrorMessage, IsLoading properties and LoginCommand
- [x] 5.3 Implement login logic: validate inputs → authenticate → navigate to role-based dashboard
- [x] 5.4 Display error message on invalid credentials
- [x] 5.5 Record LoginLog entry on successful authentication
- [x] 5.6 Apply the gray background styling matching the UI design mockup

---

## Task 6: Admin Dashboard (Window1.xaml)

- [x] 6.1 Design the Admin Dashboard with left sidebar navigation (Dashboard, Tickets, Users, Reports, Log Out)
- [x] 6.2 Add summary cards: Total Tickets, Resolved, Pending with live counts
- [x] 6.3 Add "Recent Tickets" DataGrid showing ID, Title, Status, Priority
- [x] 6.4 Create `AdminDashboardViewModel` with ticket/user counts and navigation commands
- [x] 6.5 Implement logout: update LoginLog LogoutTime, clear session, return to MainWindow

---

## Task 7: Admin Ticket Management (Window2.xaml)

- [x] 7.1 Design Ticket Management page with Search TextBox, Filter ComboBox, and Tickets DataGrid (ID, Title, Status, Priority, Assigned To)
- [x] 7.2 Add View, Update, and Assign buttons at the bottom
- [x] 7.3 Implement search and filter functionality (by status, priority, category)
- [x] 7.4 Implement Assign button: open technician selection dialog, update AssignedTo, create TicketUpdate record
- [x] 7.5 Implement ticket status update to "Closed" by Admin
- [x] 7.6 Design Users Management page with Search, Role filter, Users DataGrid (ID, Name, Username, Role, Contact)
- [x] 7.7 Add Add, Edit, Delete buttons for user management

---

## Task 8: Add/Edit User Windows (AddUser.xaml)

- [x] 8.1 Design Add User form with Full Name, Username, Password, Role (ComboBox), Contact No. fields
- [x] 8.2 Add Department ComboBox populated from Departments table
- [x] 8.3 Implement Save: validate all fields → check username uniqueness → hash password → insert User record
- [x] 8.4 Implement Edit User form pre-populated with existing user data
- [x] 8.5 Implement Update: validate fields → update User record (re-hash password if changed)
- [x] 8.6 Implement Cancel button to close window without saving

---

## Task 9: Delete Confirmation (DeleteConfirmation.xaml)

- [x] 9.1 Design confirmation dialog showing "Confirm Delete", username of user to be deleted, Delete and Cancel buttons
- [x] 9.2 Implement Delete: check for dependent tickets → delete user → refresh parent list
- [x] 9.3 Implement Cancel: close dialog without action
- [x] 9.4 Show error if user has associated tickets and cannot be deleted

---

## Task 10: Employee Dashboard (EmployeeDashboard.xaml)

- [x] 10.1 Design Employee Dashboard with left sidebar (Dashboard, My Tickets, Submit Ticket, Reports, Log Out)
- [x] 10.2 Add summary cards: My Tickets count, Resolved count, Pending count
- [x] 10.3 Add "My Recent Tickets" DataGrid showing ID, Title, Status, Priority
- [x] 10.4 Create `EmployeeDashboardViewModel` with ticket counts and navigation commands
- [x] 10.5 Load only tickets submitted by the current logged-in employee
- [x] 10.6 Implement logout functionality

---

## Task 11: Ticket Submission (TicketSubmitPage.xaml)

- [x] 11.1 Design Ticket Submission form with Title, Description (multiline), Category (ComboBox), Priority (ComboBox), Attachment (Browse button)
- [x] 11.2 Populate Category ComboBox: Hardware, Software, Network, Access, Other
- [x] 11.3 Populate Priority ComboBox: High, Medium, Low
- [x] 11.4 Implement Browse button using OpenFileDialog for attachment selection
- [x] 11.5 Implement Submit: validate required fields → create Ticket record → save attachment if provided → show confirmation
- [x] 11.6 Auto-set Status="Pending", DateSubmitted=Now, SubmittedBy=CurrentUser.UserID, DepartmentID=CurrentUser.DepartmentID

---

## Task 12: Technician Dashboard

- [x] 12.1 Design Technician Dashboard with left sidebar (Dashboard, Assigned Tickets, Update Tickets, Reports, Log Out)
- [x] 12.2 Add summary cards: Assigned Tickets, Resolved, Pending counts
- [x] 12.3 Add "Assigned Tickets" DataGrid showing ID, Title, Status, Priority
- [x] 12.4 Create `TechnicianDashboardViewModel` loading only tickets assigned to current technician
- [x] 12.5 Implement logout functionality

---

## Task 13: Ticket Update Page (Technician)

- [x] 13.1 Design Update Ticket form with Title (read-only), Description (read-only), Status (ComboBox), Remarks (TextBox), Attachment (Browse)
- [x] 13.2 Populate Status ComboBox with valid next states based on current status (enforce lifecycle)
- [x] 13.3 Implement Submit: validate Remarks not empty → update Ticket status → create TicketUpdate record → save attachment if provided
- [x] 13.4 Set DateResolved=Now when status changes to "Resolved"
- [x] 13.5 Display existing TicketUpdates history in chronological order

---

## Task 14: Assigned Tickets View (Technician)

- [x] 14.1 Design Assigned Tickets page with Search, Status filter, Priority filter, and DataGrid (ID, Title, Status, Priority, Submitted By)
- [x] 14.2 Implement search by title or ticket ID
- [x] 14.3 Implement combined filter (status + priority)
- [x] 14.4 Load only tickets where AssignedTo = CurrentUser.UserID

---

## Task 15: Reports Module (Reports.xaml / Reportss.xaml)

- [x] 15.1 Design Employee Reports page with From/To DatePicker, Category filter, Status filter, "My Ticket Summary" DataGrid (Status, Total)
- [x] 15.2 Design Admin/Technician Reports page with From/To DatePicker, Category filter, Status filter, "System Reports" DataGrid (Category, Total, Resolved, Pending)
- [x] 15.3 Implement Generate Reports button: query database with selected filters → populate DataGrid
- [x] 15.4 Calculate and display summary totals (total tickets, resolved, pending)
- [x] 15.5 Scope Employee reports to only their own tickets; Admin reports show all tickets; Technician reports show assigned tickets

---

## Task 16: Navigation and Window Management

- [x] 16.1 Implement sidebar navigation in each dashboard (highlight active page)
- [x] 16.2 Ensure secondary windows (AddUser, DeleteConfirmation) open as modal dialogs
- [x] 16.3 Prevent multiple instances of the same window from opening
- [x] 16.4 Pass current user context when navigating between windows
- [x] 16.5 Return focus to parent window when secondary windows close

---

## Task 17: Global Error Handling and Validation

- [x] 17.1 Add global `DispatcherUnhandledException` handler in `App.xaml.cs`
- [x] 17.2 Implement input validation helpers (empty field check, numeric-only for contact, date validation)
- [x] 17.3 Display validation error messages inline or via MessageBox
- [x] 17.4 Handle database connection failures with user-friendly LAN error messages
- [x] 17.5 Handle missing attachment files gracefully

---

## Task 18: Styling and UI Polish

- [ ] 18.1 Create a global `ResourceDictionary` in `App.xaml` with shared styles (buttons, text boxes, data grids) 
- [ ] 18.2 Apply consistent color scheme: primary blue `#007ACC`, sidebar dark, content area light gray
- [ ] 18.3 Apply priority color badges in DataGrids (High=Red, Medium=Orange, Low=Green)
- [ ] 18.4 Apply status color coding (Pending=Gray, In Progress=Blue, Resolved=Green, Closed=Dark)
- [ ] 18.5 Ensure all windows match the UI mockups from the design document
                                                        
---

## Task 19: LAN Deployment Configuration

- [ ] 19.1 Create `appsettings.ini` configuration file for database path (supports local and UNC network paths)
- [ ] 19.2 Implement path resolution logic in `AppSettings` class
- [ ] 19.3 Test database access via mapped network drive path
- [ ] 19.4 Ensure Attachments folder path is also configurable for network share
- [ ] 19.5 Document LAN setup instructions in a README file

---

## Task 20: Change Password Feature

- [x] 20.1 Create `ChangePassword.xaml` window with Current Password, New Password, Confirm New Password fields
- [x] 20.2 Create `ChangePasswordViewModel` with validation logic for password fields
- [x] 20.3 Add "Change Password" navigation option to all dashboards (Employee, Technician, Admin)
- [x] 20.4 Implement password validation: Current password verification, New password strength check, Confirm password match
- [x] 20.5 Add `ChangePasswordAsync` method to `IUserService` and `UserService` to update user's password
- [x] 20.6 Implement Save button: verify current password → validate new password → hash and update → show success message
- [x] 20.7 Display appropriate error messages for validation failures (wrong current password, passwords don't match, weak password)
- [x] 20.8 Ensure all users (Employee, Technician, Admin) can only change their own password

---

## Task 21: Testing and Final Verification

- [ ] 21.1 Test login with all three roles (Employee, Technician, Admin)
- [ ] 21.2 Test complete ticket lifecycle: Sbmit → Assign → In Progress → Resolved → Closed
- [ ] 21.3 Test file attachment upload and open
- [ ] 21.4 Test user CRUD operations (Add, Edit, Delete with confirmation)
- [ ] 21.5 Test report generation with date and category filters
- [ ] 21.6 Test role-based access control (verify each role cannot access restricted features)
- [ ] 21.7 Test search and filter functionality across all views
- [ ] 21.8 Verify LoginLog entries are created and updated correctly on login/logout
- [ ] 21.9 Test Change Password feature for all user roles (verify current password, password strength, confirmation match)
