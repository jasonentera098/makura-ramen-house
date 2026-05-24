# 🚨 INSTRUCTIONS: Delete Administrator Test Tickets

## Current Situation
Your runtime database has **8 tickets**:
- ❌ **5 tickets** submitted by Administrator (test tickets - WILL BE DELETED)
- ✅ **3 tickets** submitted by Employee (real tickets - WILL BE KEPT)

## Step-by-Step Instructions

### Step 1: Close the Application
**CRITICAL**: You MUST close the IT Helpdesk application completely before running the cleanup script.

1. Click the **X** button to close the application
2. Wait 5 seconds to ensure the database is released
3. Verify the application is closed (check Task Manager if needed)

### Step 2: Run the Cleanup Script
Open PowerShell in the project directory and run:

```powershell
.\DeleteAdminTicketsFromRuntime.ps1
```

### Step 3: Review the Information
The script will show you:
- Which administrator users exist
- Which tickets will be DELETED (Administrator tickets)
- Which tickets will be PRESERVED (Employee tickets)

### Step 4: Confirm Deletion
Press any key to confirm and proceed with deletion.

### Step 5: Restart the Application
After the script completes successfully, restart the IT Helpdesk application.

---

## What Will Be Deleted

These **5 Administrator test tickets** will be removed:
1. Computer won't start [Pending] [High]
2. Email not working [In Progress] [Medium]
3. Printer offline [Resolved] [Low]
4. Network connection issues [Closed] [High]
5. Software installation request [Pending] [Low]

## What Will Be Kept

These **3 Employee tickets** will be preserved:
6. Payroll System Error [Pending] [High] by Loi Acera
7. Scanner Not Detected [Pending] [Medium] by Loi Acera
8. Audio Not Working During Meeting [Pending] [Low] by Loi Acera

---

## After Cleanup

You will have:
- ✅ **3 tickets** remaining (all from Employee)
- ✅ Clean database without test tickets
- ✅ Real tickets preserved

---

## Troubleshooting

### "Database is locked" Error
**Problem**: The application is still running or the database file is in use.

**Solution**:
1. Close the IT Helpdesk application completely
2. Wait 10 seconds
3. Check Task Manager and end any "IT-Helpdesk.exe" processes
4. Run the script again

### Script Doesn't Run
**Problem**: PowerShell execution policy might be restricted.

**Solution**:
```powershell
Set-ExecutionPolicy -Scope Process -ExecutionPolicy Bypass
.\DeleteAdminTicketsFromRuntime.ps1
```

---

## Quick Command Reference

### Check Current Tickets
```powershell
.\CheckRuntimeTickets.ps1
```

### Delete Admin Tickets
```powershell
.\DeleteAdminTicketsFromRuntime.ps1
```

---

## ⚠️ IMPORTANT REMINDERS

1. **CLOSE THE APPLICATION FIRST** - This is the most important step!
2. The script will preserve Employee and Technician tickets
3. Only Administrator test tickets will be deleted
4. Restart the application after cleanup to see changes

---

**Ready to proceed?**
1. Close the application
2. Run: `.\DeleteAdminTicketsFromRuntime.ps1`
3. Restart the application
