# Manual Test Plan for Task 9.4: User Deletion Error Handling

## Test Objective
Verify that the system correctly prevents deletion of users who have associated tickets and displays appropriate error messages.

## Prerequisites
- IT Helpdesk application is running
- Database contains test data with:
  - At least one user who has submitted tickets
  - At least one user who has been assigned tickets
  - At least one user with no tickets (for control test)

## Test Cases

### Test Case 1: Delete User with Submitted Tickets

**Steps:**
1. Log in as Administrator
2. Navigate to User Management
3. Select a user who has submitted tickets
4. Click the "Delete" button
5. In the confirmation dialog, click "Delete"

**Expected Result:**
- A MessageBox appears with title "Cannot Delete User"
- Message shows: "Cannot delete user: they have submitted X ticket(s). Please reassign or close these tickets before deleting the user."
- The dialog remains open with buttons re-enabled
- User can click "Cancel" to close the dialog
- User is NOT deleted from the database

**Actual Result:** _______________

**Status:** [ ] Pass [ ] Fail

---

### Test Case 2: Delete User with Assigned Tickets

**Steps:**
1. Log in as Administrator
2. Navigate to User Management
3. Select a user (typically a Technician) who has been assigned tickets
4. Click the "Delete" button
5. In the confirmation dialog, click "Delete"

**Expected Result:**
- A MessageBox appears with title "Cannot Delete User"
- Message shows: "Cannot delete user: they have been assigned X ticket(s). Please reassign or close these tickets before deleting the user."
- The dialog remains open with buttons re-enabled
- User can click "Cancel" to close the dialog
- User is NOT deleted from the database

**Actual Result:** _______________

**Status:** [ ] Pass [ ] Fail

---

### Test Case 3: Delete User with No Tickets (Control Test)

**Steps:**
1. Log in as Administrator
2. Navigate to User Management
3. Select a user who has NO submitted or assigned tickets
4. Click the "Delete" button
5. In the confirmation dialog, click "Delete"

**Expected Result:**
- No error message appears
- The confirmation dialog closes
- User is successfully deleted from the database
- User list refreshes and the deleted user is no longer visible

**Actual Result:** _______________

**Status:** [ ] Pass [ ] Fail

---

### Test Case 4: Cancel Deletion After Error

**Steps:**
1. Log in as Administrator
2. Navigate to User Management
3. Select a user who has tickets (submitted or assigned)
4. Click the "Delete" button
5. In the confirmation dialog, click "Delete"
6. After the error message appears, click "OK" on the MessageBox
7. Click "Cancel" on the confirmation dialog

**Expected Result:**
- Error message is dismissed
- Confirmation dialog closes
- User is NOT deleted
- User list shows the user still exists

**Actual Result:** _______________

**Status:** [ ] Pass [ ] Fail

---

## Requirements Validation

This test validates the following requirements:

- **Requirement 10 (User Management)**: "WHEN an Administrator deletes a User, THE System SHALL remove the User record from the Database"
- **Requirement 30.6 (Data Integrity)**: "THE System SHALL prevent deletion of a User who has submitted or been assigned Tickets"

## Notes

- The error messages include the count of tickets to help administrators understand the scope of the dependency
- The error messages provide guidance on what action to take (reassign or close tickets)
- The UI remains responsive after the error, allowing the user to cancel the operation
