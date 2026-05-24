# Quick Cleanup Guide

## Remove Administrator Test Tickets

### One-Line Command
```powershell
.\DeleteAdminTestTickets.ps1
```

### What It Does
- ✅ Removes all tickets submitted by Administrator users
- ✅ Removes associated updates and attachments
- ✅ Keeps all user accounts intact
- ✅ Preserves tickets from Employees and Technicians

### Before Running
1. Close the IT Helpdesk application
2. Open PowerShell in the project directory

### After Running
1. Review the summary output
2. Restart the application

---

## Other Useful Commands

### Check Current Tickets
```powershell
.\CheckTickets.ps1
```

### Check Detailed Information
```powershell
.\CheckAllTicketsDetailed.ps1
```

### Remove ALL Tickets (Caution!)
```powershell
.\DeleteAllTickets.ps1
```

---

## Current Status
- **Total Tickets**: 0
- **Admin Tickets**: 0
- **Database**: Clean ✅

---

## Need Help?
See `ADMIN_TICKET_CLEANUP.md` for detailed documentation.
