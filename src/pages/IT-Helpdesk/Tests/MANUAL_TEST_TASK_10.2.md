# Manual Test for Task 10.2: Employee Dashboard Summary Cards

## Test Objective
Verify that the Employee Dashboard displays correct summary cards showing:
- My Tickets count (total tickets submitted by current employee)
- Resolved count (tickets with status "Resolved" or "Closed")
- Pending count (tickets with status "Pending" or "In Progress")

## Prerequisites
1. Application is built and running
2. Database is initialized with default data
3. Test employee user and tickets are created

## Test Setup

### Step 1: Create Test Employee User
Since only admin user exists by default, we need to:
1. Login as admin (username: `admin`, password: `admin123`)
2. Navigate to Users management
3. Create a new employee user:
   - Full Name: `John Employee`
   - Username: `employee1`
   - Password: `password123`
   - Role: `Employee`
   - Department: Any department
   - Contact: `555-0123`

### Step 2: Create Test Tickets for Employee
1. While logged in as admin, create tickets and assign SubmittedBy to the employee user ID
2. Create tickets with different statuses:
   - 2 tickets with status "Pending"
   - 1 ticket with status "In Progress"
   - 2 tickets with status "Resolved"
   - 1 ticket with status "Closed"

## Test Execution

### Test Case 1: Login as Employee
1. **Action**: Login with employee credentials (`employee1` / `password123`)
2. **Expected Result**: 
   - Employee Dashboard opens
   - Welcome message shows "Welcome, John Employee"
   - Dashboard tab is selected by default

### Test Case 2: Verify Summary Cards
1. **Action**: Observe the summary cards on the dashboard
2. **Expected Results**:
   - **My Tickets card**: Shows `6` (total tickets submitted by employee)
   - **Resolved card**: Shows `3` (tickets with "Resolved" or "Closed" status)
   - **Pending card**: Shows `3` (tickets with "Pending" or "In Progress" status)

### Test Case 3: Verify Recent Tickets Grid
1. **Action**: Scroll down to view "My Recent Tickets" section
2. **Expected Results**:
   - DataGrid shows up to 10 most recent tickets
   - Tickets are ordered by DateSubmitted (newest first)
   - Each row shows: ID, Title, Status (color-coded), Priority (color-coded)
   - Status badges use correct colors:
     - Pending: Gray
     - In Progress: Blue
     - Resolved: Green
     - Closed: Dark Gray
   - Priority badges use correct colors:
     - High: Red
     - Medium: Orange
     - Low: Green

### Test Case 4: Refresh Functionality
1. **Action**: Click the "Refresh" button
2. **Expected Result**: 
   - Loading indicator appears briefly
   - Data is reloaded and counts remain accurate

### Test Case 5: Navigation
1. **Action**: Click on different sidebar navigation items
2. **Expected Results**:
   - Dashboard: Shows summary cards and recent tickets
   - My Tickets: Shows placeholder message (Task 10.3)
   - Submit Ticket: Shows placeholder message (Task 11)
   - Reports: Shows placeholder message (Task 15)

### Test Case 6: Logout
1. **Action**: Click "Log Out" button
2. **Expected Results**:
   - LoginLog is updated with LogoutTime
   - Session is cleared
   - Login window appears
   - Employee Dashboard window closes

## Expected Summary Card Values
Based on test data:
- **My Tickets**: 6 total tickets
- **Resolved**: 3 tickets (2 "Resolved" + 1 "Closed")
- **Pending**: 3 tickets (2 "Pending" + 1 "In Progress")

## Pass/Fail Criteria
- ✅ **PASS**: All summary cards show correct counts
- ✅ **PASS**: Recent tickets grid displays properly with correct formatting
- ✅ **PASS**: Navigation works correctly
- ✅ **PASS**: Logout functionality works
- ❌ **FAIL**: Any count is incorrect or UI elements don't display properly

## Notes
- This test verifies the core functionality of Task 10.2
- The Recent Tickets grid is part of the dashboard content but the detailed "My Tickets" view will be implemented in Task 10.3
- Summary cards should update in real-time when data changes (can be tested by creating new tickets as admin)