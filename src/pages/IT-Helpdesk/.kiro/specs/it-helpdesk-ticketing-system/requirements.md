# Requirements Document

## Introduction

The IT Helpdesk Ticketing and Resolution System is a desktop-based WPF application designed to manage IT support operations within an office environment. The system enables employees to submit support tickets, IT technicians to diagnose and resolve issues, and IT managers to oversee operations and generate performance reports. The application uses MS Access as the database backend and provides role-based access control for three user types: Employees, Technicians, and Administrators.

**IMPORTANT: This is an offline, LAN-only system. The application operates entirely within the local network without any internet connectivity or online features. All data storage, file attachments, and database operations are performed locally on the LAN infrastructure.**

## Glossary

- **System**: The IT Helpdesk Ticketing and Resolution System
- **User**: Any person with credentials to access the System (Employee, Technician, or Administrator)
- **Employee**: A User with permission to submit tickets and view their own ticket status
- **Technician**: A User with permission to view, update, and resolve assigned tickets
- **Administrator**: A User with permission to manage users, assign tickets, and generate reports
- **Ticket**: A record representing an IT support request with associated metadata
- **Ticket_Lifecycle**: The progression of a Ticket through states: Pending → In Progress → Resolved → Closed
- **Priority_Level**: The urgency classification of a Ticket (High, Medium, Low)
- **Database**: The MS Access database storing all System data
- **Session**: The period between a User's successful login and logout
- **Attachment**: A file associated with a Ticket for troubleshooting purposes
- **Dashboard**: The main interface displaying relevant information based on User role
- **Ticket_Update**: A record of changes or remarks added to a Ticket
- **Login_Log**: A record of User authentication events with timestamps
- **Department**: An organizational unit to which Users and Tickets are associated
- **Category**: The classification of a Ticket by issue type (Hardware, Software, Network, etc.)
- **Status**: The current state of a Ticket in the Ticket_Lifecycle
- **Report**: A filtered and formatted view of System data for analysis purposes

## Requirements

### Requirement 1: User Authentication

**User Story:** As a User, I want to authenticate with my credentials, so that I can access the System securely based on my role.

#### Acceptance Criteria

1. WHEN a User enters valid credentials, THE System SHALL authenticate the User and grant access to the appropriate Dashboard
2. WHEN a User enters invalid credentials, THE System SHALL display an error message and deny access
3. THE System SHALL hash passwords before storing them in the Database
4. WHEN a User successfully authenticates, THE System SHALL create a Login_Log entry with LoginTime
5. THE System SHALL validate that Username and Password fields are not empty before authentication
6. WHEN authentication succeeds, THE System SHALL load the User's role from the Database
7. THE System SHALL prevent access to the System until authentication succeeds

### Requirement 2: Session Management

**User Story:** As a User, I want my session to be tracked, so that the System maintains security and audit trails.

#### Acceptance Criteria

1. WHILE a User is authenticated, THE System SHALL maintain an active Session
2. WHEN a User logs out, THE System SHALL update the Login_Log entry with LogoutTime
3. WHEN a User logs out, THE System SHALL terminate the Session and return to the login screen
4. THE System SHALL store the current User's UserID and Role in memory during the Session
5. WHEN the application closes without explicit logout, THE System SHALL update the Login_Log with the application close time

### Requirement 3: Role-Based Access Control

**User Story:** As an Administrator, I want users to have different permissions based on their roles, so that the System maintains appropriate access restrictions.

#### Acceptance Criteria

1. WHEN an Employee authenticates, THE System SHALL display the Employee Dashboard with ticket submission and tracking features
2. WHEN a Technician authenticates, THE System SHALL display the Technician Dashboard with assigned tickets and update capabilities
3. WHEN an Administrator authenticates, THE System SHALL display the Administrator Dashboard with user management, ticket assignment, and reporting features
4. THE System SHALL prevent Users from accessing features not permitted for their Role
5. THE System SHALL load UI elements dynamically based on the authenticated User's Role

### Requirement 4: Ticket Submission

**User Story:** As an Employee, I want to submit support tickets, so that I can request IT assistance for issues.

#### Acceptance Criteria

1. WHEN an Employee submits a ticket, THE System SHALL create a Ticket record with Title, Description, Category, Priority, Status set to "Pending", DateSubmitted, and SubmittedBy
2. THE System SHALL validate that Title and Description fields are not empty before submission
3. THE System SHALL validate that Category and Priority are selected from predefined options
4. WHEN a Ticket is created, THE System SHALL associate it with the Employee's DepartmentID
5. WHEN a Ticket is successfully created, THE System SHALL display a confirmation message
6. THE System SHALL assign a unique TicketID to each new Ticket
7. WHEN a Ticket is created, THE System SHALL set AssignedTo to null until assignment occurs

### Requirement 5: Ticket Assignment

**User Story:** As an Administrator, I want to assign tickets to technicians, so that issues are distributed for resolution.

#### Acceptance Criteria

1. WHEN an Administrator assigns a Ticket, THE System SHALL update the AssignedTo field with the selected Technician's UserID
2. THE System SHALL display only Tickets with Status "Pending" or "In Progress" in the assignment interface
3. THE System SHALL display only Users with Role "Technician" in the assignment selection
4. WHEN a Ticket is assigned, THE System SHALL create a Ticket_Update record with the assignment details
5. THE System SHALL validate that a Technician is selected before allowing assignment
6. WHEN assignment succeeds, THE System SHALL display a confirmation message

### Requirement 6: Ticket Status Progression

**User Story:** As a Technician, I want to update ticket status, so that I can track progress through the resolution lifecycle.

#### Acceptance Criteria

1. WHEN a Technician updates a Ticket to "In Progress", THE System SHALL change the Status from "Pending" to "In Progress"
2. WHEN a Technician updates a Ticket to "Resolved", THE System SHALL change the Status to "Resolved" and set DateResolved to the current timestamp
3. WHEN an Administrator updates a Ticket to "Closed", THE System SHALL change the Status from "Resolved" to "Closed"
4. THE System SHALL enforce the Ticket_Lifecycle progression: Pending → In Progress → Resolved → Closed
5. THE System SHALL prevent status changes that violate the Ticket_Lifecycle order
6. WHEN a status change occurs, THE System SHALL create a Ticket_Update record with the change details

### Requirement 7: Ticket Updates and Remarks

**User Story:** As a Technician, I want to add remarks to tickets, so that I can document troubleshooting steps and findings.

#### Acceptance Criteria

1. WHEN a Technician adds a remark, THE System SHALL create a Ticket_Update record with UpdatedBy, Remarks, and UpdateDate
2. THE System SHALL validate that the Remarks field is not empty before creating the update
3. THE System SHALL associate the Ticket_Update with the correct TicketID
4. THE System SHALL display all Ticket_Updates for a Ticket in chronological order
5. THE System SHALL include the User's FullName when displaying Ticket_Updates

### Requirement 8: Attachment Management

**User Story:** As a User, I want to attach files to tickets, so that I can provide additional context for troubleshooting.

#### Acceptance Criteria

1. WHEN a User attaches a file to a Ticket, THE System SHALL create an Attachment record with AttachmentID, TicketID, FilePath, UploadedBy, and UploadedAt
2. THE System SHALL copy the selected file to a local storage directory
3. THE System SHALL store the file path in the Attachment record
4. THE System SHALL validate that a file is selected before allowing attachment
5. WHEN an attachment is added, THE System SHALL display a confirmation message
6. THE System SHALL display all Attachments associated with a Ticket
7. WHEN a User selects an Attachment, THE System SHALL open the file using the default system application

### Requirement 9: Ticket Filtering and Search

**User Story:** As a User, I want to filter and search tickets, so that I can quickly find relevant information.

#### Acceptance Criteria

1. WHEN a User enters search criteria, THE System SHALL filter displayed Tickets based on Title, Description, or TicketID
2. THE System SHALL support filtering by Status, Priority, and Category
3. THE System SHALL support filtering by date range using DateSubmitted
4. THE System SHALL display filtered results in the DataGrid control
5. WHEN no Tickets match the filter criteria, THE System SHALL display an empty result set with a message
6. THE System SHALL apply filters in combination when multiple criteria are specified

### Requirement 10: User Management

**User Story:** As an Administrator, I want to manage user accounts, so that I can control system access and maintain user information.

#### Acceptance Criteria

1. WHEN an Administrator creates a User, THE System SHALL insert a record with FullName, Username, Password, Role, DepartmentID, ContactNumber, and CreatedAt
2. THE System SHALL validate that Username is unique before creating a User
3. THE System SHALL validate that all required fields are populated before creating a User
4. WHEN an Administrator updates a User, THE System SHALL modify the existing User record
5. WHEN an Administrator deletes a User, THE System SHALL remove the User record from the Database
6. THE System SHALL display all Users in a DataGrid with sorting capabilities
7. THE System SHALL hash passwords before storing them during User creation or password updates

### Requirement 11: Department Management

**User Story:** As an Administrator, I want to manage departments, so that I can organize users and tickets by organizational units.

#### Acceptance Criteria

1. WHEN an Administrator creates a Department, THE System SHALL insert a record with DepartmentName and Description
2. THE System SHALL validate that DepartmentName is unique before creating a Department
3. THE System SHALL display all Departments in a selection control when assigning Users or Tickets
4. WHEN an Administrator updates a Department, THE System SHALL modify the existing Department record
5. THE System SHALL prevent deletion of a Department that has associated Users or Tickets

### Requirement 12: Technician Performance Dashboard

**User Story:** As a Technician, I want to view my performance metrics, so that I can track my productivity and workload.

#### Acceptance Criteria

1. THE System SHALL display the count of Tickets assigned to the authenticated Technician with Status "Pending"
2. THE System SHALL display the count of Tickets assigned to the authenticated Technician with Status "In Progress"
3. THE System SHALL display the count of Tickets assigned to the authenticated Technician with Status "Resolved"
4. THE System SHALL calculate and display the total number of Tickets assigned to the authenticated Technician
5. THE System SHALL display Tickets assigned to the authenticated Technician in a DataGrid
6. THE System SHALL update Dashboard metrics when Ticket statuses change

### Requirement 13: Report Generation

**User Story:** As an Administrator, I want to generate reports, so that I can analyze system usage and performance trends.

#### Acceptance Criteria

1. WHEN an Administrator requests a report, THE System SHALL retrieve Tickets based on selected filter criteria
2. THE System SHALL support filtering reports by date range using DateSubmitted
3. THE System SHALL support filtering reports by Status, Priority, Category, and Department
4. THE System SHALL display report results in a DataGrid control
5. THE System SHALL calculate and display summary statistics (total tickets, resolved tickets, pending tickets)
6. WHEN no Tickets match the report criteria, THE System SHALL display an empty result set with a message

### Requirement 14: Priority-Based Ticket Handling

**User Story:** As a User, I want tickets to be prioritized, so that urgent issues receive appropriate attention.

#### Acceptance Criteria

1. THE System SHALL support three Priority_Levels: High, Medium, and Low
2. WHEN displaying Tickets, THE System SHALL support sorting by Priority_Level
3. THE System SHALL display Priority_Level for each Ticket in list views
4. WHEN a Ticket is created or updated, THE System SHALL validate that Priority_Level is one of the three allowed values
5. THE System SHALL allow Users to filter Tickets by Priority_Level

### Requirement 15: Data Validation and Input Constraints

**User Story:** As a User, I want the system to validate my input, so that I can avoid errors and maintain data integrity.

#### Acceptance Criteria

1. WHEN a User submits a form, THE System SHALL validate that required fields are not empty
2. WHEN a User enters a date, THE System SHALL validate that the date format is correct
3. WHEN a User enters a ContactNumber, THE System SHALL validate that it contains only numeric characters
4. WHEN validation fails, THE System SHALL display an error message indicating the specific validation issue
5. THE System SHALL prevent form submission when validation fails
6. THE System SHALL provide visual indicators for required fields

### Requirement 16: Database Connection Management

**User Story:** As a User, I want the system to manage database connections reliably, so that I can perform operations without connection errors.

#### Acceptance Criteria

1. WHEN the System starts, THE System SHALL establish a connection to the MS Access Database
2. WHEN a database operation is requested, THE System SHALL verify the connection is active
3. IF the database connection fails, THEN THE System SHALL display an error message and prevent data operations
4. WHEN a database operation completes, THE System SHALL close the connection to release resources
5. THE System SHALL use parameterized queries to prevent SQL injection vulnerabilities

### Requirement 17: Ticket Tracking for Employees

**User Story:** As an Employee, I want to track my submitted tickets, so that I can monitor the status of my support requests.

#### Acceptance Criteria

1. THE System SHALL display all Tickets where SubmittedBy matches the authenticated Employee's UserID
2. THE System SHALL display Ticket details including Title, Description, Category, Priority, Status, DateSubmitted, and AssignedTo
3. THE System SHALL display Ticket_Updates associated with each Ticket
4. THE System SHALL display Attachments associated with each Ticket
5. WHEN an Employee selects a Ticket, THE System SHALL display the complete Ticket history
6. THE System SHALL refresh the Ticket list when the Employee navigates to the tracking view

### Requirement 18: Navigation and Window Management

**User Story:** As a User, I want to navigate between different views, so that I can access various system features.

#### Acceptance Criteria

1. WHEN a User selects a navigation option, THE System SHALL display the corresponding view or window
2. THE System SHALL maintain the current Session context when navigating between views
3. WHEN a User closes a secondary window, THE System SHALL return focus to the parent window
4. THE System SHALL prevent multiple instances of the same window from opening simultaneously
5. THE System SHALL display navigation options appropriate for the User's Role

### Requirement 19: Data Binding and UI Updates

**User Story:** As a User, I want the interface to reflect current data, so that I can make decisions based on accurate information.

#### Acceptance Criteria

1. WHEN data changes in the Database, THE System SHALL update the UI to reflect the changes
2. THE System SHALL use ObservableCollection for data-bound lists to enable automatic UI updates
3. WHEN a User performs a create, update, or delete operation, THE System SHALL refresh the affected DataGrid
4. THE System SHALL bind form controls to data models using WPF data binding
5. THE System SHALL display loading indicators during data retrieval operations

### Requirement 20: Error Handling and User Feedback

**User Story:** As a User, I want clear feedback on operations, so that I understand whether actions succeeded or failed.

#### Acceptance Criteria

1. WHEN an operation succeeds, THE System SHALL display a success message
2. WHEN an operation fails, THE System SHALL display an error message with a description of the failure
3. IF a database error occurs, THEN THE System SHALL log the error details and display a user-friendly message
4. THE System SHALL use MessageBox or similar controls to display feedback messages
5. WHEN a long-running operation is in progress, THE System SHALL display a loading indicator
6. THE System SHALL clear error messages when the User corrects the input and resubmits

### Requirement 21: Ticket Category Management

**User Story:** As an Administrator, I want to define ticket categories, so that issues can be classified consistently.

#### Acceptance Criteria

1. THE System SHALL support predefined Categories including Hardware, Software, Network, Access, and Other
2. WHEN a User creates or updates a Ticket, THE System SHALL display Categories in a ComboBox control
3. THE System SHALL validate that a Category is selected before allowing Ticket submission
4. THE System SHALL store the Category value in the Ticket record
5. THE System SHALL support filtering Tickets by Category

### Requirement 22: Audit Trail and Logging

**User Story:** As an Administrator, I want to track user activities, so that I can maintain accountability and troubleshoot issues.

#### Acceptance Criteria

1. WHEN a User logs in, THE System SHALL create a Login_Log entry with UserID and LoginTime
2. WHEN a User logs out, THE System SHALL update the Login_Log entry with LogoutTime
3. WHEN a Ticket status changes, THE System SHALL create a Ticket_Update record documenting the change
4. THE System SHALL store the UserID of the User who performed each action
5. THE System SHALL display Login_Logs in the Administrator Dashboard with filtering by date range

### Requirement 23: Confirmation Dialogs for Destructive Actions

**User Story:** As a User, I want confirmation prompts for destructive actions, so that I can avoid accidental data loss.

#### Acceptance Criteria

1. WHEN a User attempts to delete a record, THE System SHALL display a confirmation dialog
2. WHEN a User confirms deletion, THE System SHALL proceed with the delete operation
3. WHEN a User cancels deletion, THE System SHALL abort the operation and return to the previous view
4. THE System SHALL display the record details in the confirmation dialog
5. THE System SHALL use a modal dialog to prevent interaction with other windows during confirmation

### Requirement 24: Date and Time Handling

**User Story:** As a User, I want dates and times to be recorded accurately, so that I can track when events occurred.

#### Acceptance Criteria

1. WHEN a Ticket is created, THE System SHALL set DateSubmitted to the current date and time
2. WHEN a Ticket is resolved, THE System SHALL set DateResolved to the current date and time
3. WHEN a Ticket_Update is created, THE System SHALL set UpdateDate to the current date and time
4. WHEN an Attachment is uploaded, THE System SHALL set UploadedAt to the current date and time
5. THE System SHALL display dates in a consistent format throughout the UI
6. THE System SHALL use DatePicker controls for date input fields

### Requirement 25: Password Security

**User Story:** As an Administrator, I want passwords to be stored securely, so that user credentials are protected.

#### Acceptance Criteria

1. WHEN a User is created, THE System SHALL hash the password using a cryptographic hash function before storing it
2. WHEN a User authenticates, THE System SHALL hash the entered password and compare it to the stored hash
3. THE System SHALL never display passwords in plain text in the UI
4. THE System SHALL use PasswordBox controls for password input
5. WHEN a User updates their password, THE System SHALL hash the new password before storing it

### Requirement 26: File Storage Management

**User Story:** As a User, I want attachments to be stored reliably, so that I can access them when needed.

#### Acceptance Criteria

1. WHEN a file is attached, THE System SHALL copy the file to a designated local storage directory
2. THE System SHALL organize stored files by TicketID to prevent naming conflicts
3. THE System SHALL store the complete file path in the Attachment record
4. WHEN a User opens an attachment, THE System SHALL verify the file exists before attempting to open it
5. IF a file does not exist, THEN THE System SHALL display an error message

### Requirement 27: Ticket Assignment Validation

**User Story:** As an Administrator, I want to ensure tickets are assigned correctly, so that workload is distributed appropriately.

#### Acceptance Criteria

1. THE System SHALL prevent assignment of a Ticket to a User with Role "Employee"
2. THE System SHALL allow assignment of a Ticket to a User with Role "Technician"
3. THE System SHALL allow reassignment of a Ticket to a different Technician
4. WHEN a Ticket is reassigned, THE System SHALL create a Ticket_Update record documenting the reassignment
5. THE System SHALL display the currently assigned Technician's FullName in the Ticket details view

### Requirement 28: Search Result Relevance

**User Story:** As a User, I want search results to be relevant, so that I can find information quickly.

#### Acceptance Criteria

1. WHEN a User searches by TicketID, THE System SHALL perform an exact match search
2. WHEN a User searches by Title or Description, THE System SHALL perform a case-insensitive partial match search
3. THE System SHALL display search results sorted by DateSubmitted in descending order
4. THE System SHALL highlight or indicate which fields matched the search criteria
5. WHEN a User clears the search, THE System SHALL display all Tickets according to the User's Role permissions

### Requirement 29: UI Responsiveness and Performance

**User Story:** As a User, I want the interface to respond quickly, so that I can work efficiently.

#### Acceptance Criteria

1. WHEN a User performs a database query, THE System SHALL return results within 2 seconds for datasets under 1000 records
2. THE System SHALL use asynchronous operations for database queries to prevent UI freezing
3. WHEN loading large datasets, THE System SHALL display a progress indicator
4. THE System SHALL implement pagination or lazy loading for DataGrids displaying more than 100 records
5. THE System SHALL cache frequently accessed data to reduce database queries

### Requirement 30: Data Integrity and Referential Constraints

**User Story:** As an Administrator, I want data relationships to be maintained, so that the database remains consistent.

#### Acceptance Criteria

1. THE System SHALL enforce that every Ticket has a valid SubmittedBy UserID
2. THE System SHALL enforce that every Ticket has a valid DepartmentID
3. WHEN a Ticket is assigned, THE System SHALL validate that AssignedTo references a valid UserID
4. THE System SHALL enforce that every Attachment has a valid TicketID
5. THE System SHALL enforce that every Ticket_Update has a valid TicketID and UpdatedBy UserID
6. THE System SHALL prevent deletion of a User who has submitted or been assigned Tickets
7. THE System SHALL prevent deletion of a Department that has associated Users or Tickets

### Requirement 31: Offline and LAN-Only Operation

**User Story:** As a System Administrator, I want the system to operate entirely offline within the local network, so that we maintain data security and independence from internet connectivity.

#### Acceptance Criteria

1. THE System SHALL operate entirely within the local area network (LAN) without requiring internet connectivity
2. THE System SHALL NOT attempt to connect to any external online services or APIs
3. THE System SHALL store the MS Access database file on a shared network location accessible via LAN
4. THE System SHALL store all file attachments on local or network-shared storage within the LAN
5. THE System SHALL NOT include any features that require internet access (email notifications, cloud storage, web APIs, etc.)
6. THE System SHALL function normally when the network has no internet connection
7. WHEN the System cannot access the network database, THE System SHALL display an appropriate error message indicating network connectivity issues (not internet issues)
8. THE System SHALL use UNC paths or mapped network drives for accessing shared resources on the LAN

