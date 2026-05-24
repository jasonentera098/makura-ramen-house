# DeleteAdminTicketsFromRuntime.ps1
# Deletes ONLY Administrator tickets from the RUNTIME database
# Preserves Employee and Technician tickets

$dbPath = "bin\Debug\net8.0-windows\Data\HelpdeskDB.accdb"

if (-not (Test-Path $dbPath)) {
    Write-Host "Runtime database not found at: $dbPath" -ForegroundColor Red
    exit 1
}

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "Delete Administrator Test Tickets" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""
Write-Host "Target: RUNTIME database" -ForegroundColor Yellow
Write-Host "Path: $dbPath" -ForegroundColor Gray
Write-Host ""
Write-Host "This will delete ONLY tickets submitted by Administrator users." -ForegroundColor Yellow
Write-Host "Employee and Technician tickets will be preserved." -ForegroundColor Green
Write-Host ""
Write-Host "CRITICAL: Close the IT Helpdesk application NOW!" -ForegroundColor Red
Write-Host "Press any key to continue or Ctrl+C to cancel..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Write-Host ""

$connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath"
$connection = New-Object System.Data.OleDb.OleDbConnection($connectionString)

try {
    $connection.Open()
    Write-Host "Connected to runtime database!" -ForegroundColor Green
    Write-Host ""

    # Get all admin user IDs
    $getAdminsCmd = $connection.CreateCommand()
    $getAdminsCmd.CommandText = "SELECT UserID, Username, FullName FROM Users WHERE Role = 'Administrator'"
    $adminReader = $getAdminsCmd.ExecuteReader()
    
    $adminUserIds = @()
    $adminInfo = @()
    while ($adminReader.Read()) {
        $userId = $adminReader["UserID"]
        $username = $adminReader["Username"]
        $fullName = $adminReader["FullName"]
        $adminUserIds += $userId
        $adminInfo += "  - $fullName (@$username) [ID: $userId]"
    }
    $adminReader.Close()
    
    if ($adminUserIds.Count -eq 0) {
        Write-Host "No administrator users found." -ForegroundColor Yellow
        exit 0
    }

    Write-Host "Found $($adminUserIds.Count) administrator user(s):" -ForegroundColor Cyan
    $adminInfo | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
    Write-Host ""

    # Build the WHERE clause
    $adminIdList = $adminUserIds -join ','

    # Get tickets to delete
    $getTicketsCmd = $connection.CreateCommand()
    $getTicketsCmd.CommandText = @"
SELECT t.TicketID, t.Title, t.Status, t.Priority, u.Username 
FROM Tickets t 
INNER JOIN Users u ON t.SubmittedBy = u.UserID 
WHERE t.SubmittedBy IN ($adminIdList)
ORDER BY t.TicketID
"@
    $ticketReader = $getTicketsCmd.ExecuteReader()
    
    $ticketIdsToDelete = @()
    Write-Host "Administrator tickets to be deleted:" -ForegroundColor Yellow
    Write-Host ("=" * 100)
    while ($ticketReader.Read()) {
        $ticketId = $ticketReader["TicketID"]
        $title = $ticketReader["Title"]
        $status = $ticketReader["Status"]
        $priority = $ticketReader["Priority"]
        $username = $ticketReader["Username"]
        $ticketIdsToDelete += $ticketId
        Write-Host ("  #{0,-3} {1,-40} [{2}] [{3}] by @{4}" -f $ticketId, $title, $status, $priority, $username) -ForegroundColor Gray
    }
    $ticketReader.Close()
    Write-Host ("=" * 100)
    Write-Host ""

    if ($ticketIdsToDelete.Count -eq 0) {
        Write-Host "No administrator tickets to delete!" -ForegroundColor Green
        exit 0
    }

    Write-Host "Total to delete: $($ticketIdsToDelete.Count) ticket(s)" -ForegroundColor Yellow
    Write-Host ""

    # Show what will be kept
    $getKeepTicketsCmd = $connection.CreateCommand()
    $getKeepTicketsCmd.CommandText = @"
SELECT t.TicketID, t.Title, u.Username, u.Role
FROM Tickets t 
INNER JOIN Users u ON t.SubmittedBy = u.UserID 
WHERE t.SubmittedBy NOT IN ($adminIdList)
ORDER BY t.TicketID
"@
    $keepReader = $getKeepTicketsCmd.ExecuteReader()
    
    $keepTickets = @()
    while ($keepReader.Read()) {
        $keepTickets += [PSCustomObject]@{
            ID = $keepReader["TicketID"]
            Title = $keepReader["Title"]
            Username = $keepReader["Username"]
            Role = $keepReader["Role"]
        }
    }
    $keepReader.Close()

    if ($keepTickets.Count -gt 0) {
        Write-Host "Tickets that will be PRESERVED:" -ForegroundColor Green
        Write-Host ("=" * 100)
        foreach ($ticket in $keepTickets) {
            Write-Host ("  #{0,-3} {1,-40} by @{2} [{3}]" -f $ticket.ID, $ticket.Title, $ticket.Username, $ticket.Role) -ForegroundColor Green
        }
        Write-Host ("=" * 100)
        Write-Host ""
    }

    Write-Host "Deleting related records..." -ForegroundColor Cyan

    $totalUpdatesDeleted = 0
    $totalAttachmentsDeleted = 0

    # Delete related TicketUpdates
    foreach ($ticketId in $ticketIdsToDelete) {
        $deleteUpdatesCmd = $connection.CreateCommand()
        $deleteUpdatesCmd.CommandText = "DELETE FROM TicketUpdates WHERE TicketID = ?"
        $deleteUpdatesCmd.Parameters.AddWithValue("@TicketID", $ticketId) | Out-Null
        $updatesDeleted = $deleteUpdatesCmd.ExecuteNonQuery()
        $totalUpdatesDeleted += $updatesDeleted
    }

    # Delete related Attachments
    foreach ($ticketId in $ticketIdsToDelete) {
        $deleteAttachmentsCmd = $connection.CreateCommand()
        $deleteAttachmentsCmd.CommandText = "DELETE FROM Attachments WHERE TicketID = ?"
        $deleteAttachmentsCmd.Parameters.AddWithValue("@TicketID", $ticketId) | Out-Null
        $attachmentsDeleted = $deleteAttachmentsCmd.ExecuteNonQuery()
        $totalAttachmentsDeleted += $attachmentsDeleted
    }

    # Delete the tickets
    Write-Host "Deleting administrator tickets..." -ForegroundColor Cyan
    $deleteTicketsCmd = $connection.CreateCommand()
    $deleteTicketsCmd.CommandText = "DELETE FROM Tickets WHERE SubmittedBy IN ($adminIdList)"
    $ticketsDeleted = $deleteTicketsCmd.ExecuteNonQuery()

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "Cleanup Complete!" -ForegroundColor Green
    Write-Host "========================================" -ForegroundColor Green
    Write-Host "  Administrator tickets deleted: $ticketsDeleted" -ForegroundColor Green
    Write-Host "  Updates deleted:               $totalUpdatesDeleted" -ForegroundColor Green
    Write-Host "  Attachments deleted:           $totalAttachmentsDeleted" -ForegroundColor Green
    Write-Host ""

    # Show final count
    $remainingCmd = $connection.CreateCommand()
    $remainingCmd.CommandText = "SELECT COUNT(*) FROM Tickets"
    $remainingCount = [int]$remainingCmd.ExecuteScalar()
    Write-Host "  Remaining tickets: $remainingCount" -ForegroundColor Cyan

    # Show breakdown
    if ($remainingCount -gt 0) {
        Write-Host ""
        Write-Host "Remaining tickets by role:" -ForegroundColor Cyan
        $breakdownCmd = $connection.CreateCommand()
        $breakdownCmd.CommandText = @"
SELECT u.Role, COUNT(t.TicketID) as TicketCount 
FROM Tickets t 
INNER JOIN Users u ON t.SubmittedBy = u.UserID 
GROUP BY u.Role
"@
        $breakdownReader = $breakdownCmd.ExecuteReader()
        while ($breakdownReader.Read()) {
            $role = $breakdownReader["Role"]
            $count = $breakdownReader["TicketCount"]
            Write-Host "  $role : $count ticket(s)" -ForegroundColor Green
        }
        $breakdownReader.Close()
    }

    Write-Host ""
    Write-Host "========================================" -ForegroundColor Yellow
    Write-Host "IMPORTANT: Restart the application now!" -ForegroundColor Yellow
    Write-Host "========================================" -ForegroundColor Yellow

} catch {
    Write-Host ""
    Write-Host "ERROR: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    if ($_.Exception.Message -like "*database is locked*" -or $_.Exception.Message -like "*being used by another*") {
        Write-Host "The database is currently LOCKED!" -ForegroundColor Red
        Write-Host ""
        Write-Host "You MUST close the IT Helpdesk application first:" -ForegroundColor Yellow
        Write-Host "  1. Close the application completely" -ForegroundColor Yellow
        Write-Host "  2. Wait a few seconds" -ForegroundColor Yellow
        Write-Host "  3. Run this script again" -ForegroundColor Yellow
    }
    exit 1
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host ""
Write-Host "Done!" -ForegroundColor Green
Write-Host ""
