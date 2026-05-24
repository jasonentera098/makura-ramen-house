# Administrator Test Ticket Cleanup

## Overview
This document explains how to remove test/example tickets that were submitted by Administrator users from the IT Helpdesk database.

## Why Remove Admin Test Tickets?
Administrator accounts are typically used for testing and creating example tickets during development. These tickets are not real support requests and should be removed before production use or when you want to start with a clean slate.

## What Gets Deleted?
The cleanup script removes:
- ✅ All tickets submitted by users with the "Administrator" role
- ✅ All ticket updates associated with those tickets
- ✅ All attachments associated with those tickets

## What Stays?
The script preserves:
- ✅ All tickets submitted by Employees
- ✅ All tickets submitted by Technicians
- ✅ All user accounts (including administrators)
- ✅ All departments and other system data

## How to Use

### Method 1: Run the Cleanup Script (Recommended)

1. **Close the IT Helpdesk application** (important!)

2. **Open PowerShell** in the project directory

3. **Run the cleanup script:**
   ```powershell
   .\DeleteAdminTestTickets.ps1
   ```

4. **Review the information** displayed:
   - Which databases will be cleaned
   - Which administrator users exist
   - How many tickets will be deleted

5. **Press any key to confirm** and proceed with deletion

6. **Restart the application** to see the changes

### Method 2: Manual Database Query

If you prefer to manually delete tickets, you can use Microsoft Access or a database tool:

```sql
-- Delete ticket updates first (foreign key constraint)
DELETE FROM TicketUpdates 
WHERE TicketID IN (
    SELECT TicketID FROM Tickets 
    WHERE SubmittedBy IN (
        SELECT UserID FROM Users WHERE Role = 'Administrator'
    )
);

-- Delete attachments
DELETE FROM Attachments 
WHERE TicketID IN (
    SELECT TicketID FROM Tickets 
    WHERE SubmittedBy IN (
        SELECT UserID FROM Users WHERE Role = 'Administrator'
    )
);

-- Delete the tickets
DELETE FROM Tickets 
WHERE SubmittedBy IN (
    SELECT UserID FROM Users WHERE Role = 'Administrator'
);
```

## Script Features

### Multi-Database Support
The script automatically detects and cleans both:
- **Source Database**: `Data\HelpdeskDB.accdb`
- **Runtime Database**: `bin\Debug\net8.0-windows\Data\HelpdeskDB.accdb`

### Safety Features
- ✅ Requires confirmation before deletion
- ✅ Shows detailed information about what will be deleted
- ✅ Displays summary of deleted items
- ✅ Shows remaining ticket count and breakdown by role
- ✅ Handles database lock errors gracefully

### Output Information
The script displays:
- Administrator users found
- List of tickets to be deleted (ID, Title, Status, Submitter)
- Number of updates deleted
- Number of attachments deleted
- Number of tickets deleted
- Remaining ticket count
- Breakdown of remaining tickets by submitter role

## Example Output

```
Found 2 database(s) to clean:
  - Source Database: Data\HelpdeskDB.accdb
  - Runtime Database: bin\Debug\net8.0-windows\Data\HelpdeskDB.accdb

========================================
Processing: Source Database
========================================

Found 1 administrator user(s):
  - System Administrator (@Administrator) [ID: 1]

Found 5 ticket(s) submitted by administrators

Tickets to be deleted:
  #1 - Computer won't start [Pending] (by @Administrator)
  #2 - Email not working [In Progress] (by @Administrator)
  #3 - Printer offline [Resolved] (by @Administrator)
  #4 - Network connection issues [Closed] (by @Administrator)
  #5 - Software installation request [Pending] (by @Administrator)

Cleanup Complete for Source Database!
  Tickets deleted:      5
  Updates deleted:      0
  Attachments deleted:  0

  Remaining tickets: 6

Remaining tickets by submitter role:
  Employee : 6 ticket(s)
```

## Related Scripts

### Other Cleanup Scripts Available:
- **`CleanupSampleTickets.ps1`** - Removes tickets without valid SubmittedBy users
- **`DeleteAllTickets.ps1`** - Removes ALL tickets (use with extreme caution!)
- **`DeleteInvalidTicketsFromRuntime.ps1`** - Removes invalid tickets from runtime database only

### Verification Scripts:
- **`CheckTickets.ps1`** - View all tickets in the database
- **`CheckAllTicketsDetailed.ps1`** - View detailed ticket information
- **`CheckRuntimeDatabase.ps1`** - Check runtime database status

## Troubleshooting

### "Database is locked" Error
**Problem**: The database file is currently in use by the application.

**Solution**: 
1. Close the IT Helpdesk application completely
2. Run the script again

### "No databases found" Error
**Problem**: The script cannot find the database files.

**Solution**:
1. Make sure you're running the script from the project root directory
2. Check if the database files exist in the expected locations
3. Build the application first if the runtime database doesn't exist

### Script Doesn't Delete Anything
**Problem**: The script reports "No admin tickets to delete!"

**Solution**: This is normal if:
- You've already run the cleanup
- No administrator users have submitted tickets
- All tickets were submitted by Employees or Technicians

## Best Practices

1. **Always close the application** before running database cleanup scripts
2. **Backup your database** before running cleanup scripts (optional but recommended)
3. **Review the output** to ensure the correct tickets are being deleted
4. **Restart the application** after cleanup to refresh the data

## Production Deployment

Before deploying to production:
1. Run this cleanup script to remove all test tickets
2. Verify that only legitimate tickets remain
3. Consider creating a fresh database with only the necessary user accounts
4. Document which administrator accounts should exist in production

## Notes

- The script does NOT delete user accounts, only tickets
- Administrator accounts remain active after cleanup
- The script is safe to run multiple times
- No data is deleted from other tables (Departments, LoginLogs, etc.)

## Support

If you encounter issues with the cleanup script:
1. Check the error message displayed
2. Ensure the application is closed
3. Verify database file permissions
4. Check that Microsoft Access Database Engine is installed

---

**Last Updated**: May 16, 2026
**Script Version**: 1.0
