# DeleteAdminTestTickets.ps1
# Removes all tickets submitted by Administrator users (test/example tickets)
# This script checks both source and runtime databases

$sourcePath = "Data\HelpdeskDB.accdb"
$runtimePath = "bin\Debug\net8.0-windows\Data\HelpdeskDB.accdb"

$databasesToClean = @()

if (Test-Path $sourcePath) {
    $databasesToClean += @{ Path = $sourcePath; Name = "Source Database" }
}

if (Test-Path $runtimePath) {
    $databasesToClean += @{ Path = $runtimePath; Name = "Runtime Database" }
}

if ($databasesToClean.Count -eq 0) {
    Write-Host "No databases found!" -ForegroundColor Red
    Write-Host "  Checked: $sourcePath" -ForegroundColor Gray
    Write-Host "  Checked: $runtimePath" -ForegroundColor Gray
    exit 1
}

Write-Host "Found $($databasesToClean.Count) database(s) to clean:" -ForegroundColor Cyan
foreach ($db in $databasesToClean) {
    Write-Host "  - $($db.Name): $($db.Path)" -ForegroundColor Gray
}
Write-Host ""
Write-Host "This will delete all tickets submitted by Administrator users." -ForegroundColor Yellow
Write-Host "These are typically test/example tickets." -ForegroundColor Yellow
Write-Host ""
Write-Host "IMPORTANT: Close the IT Helpdesk application before running this script!" -ForegroundColor Yellow
Write-Host "Press any key to continue or Ctrl+C to cancel..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Write-Host ""

function Clean-Database {
    param (
        [string]$dbPath,
        [string]$dbName
    )

    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Processing: $dbName" -ForegroundColor Cyan
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "Path: $dbPath" -ForegroundColor Gray
    Write-Host ""

    $connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath"
    $connection = New-Object System.Data.OleDb.OleDbConnection($connectionString)

    try {
        $connection.Open()
        Write-Host "Connected successfully!" -ForegroundColor Green
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
            return
        }

        Write-Host "Found $($adminUserIds.Count) administrator user(s):" -ForegroundColor Cyan
        $adminInfo | ForEach-Object { Write-Host $_ -ForegroundColor Gray }
        Write-Host ""

        # Build the WHERE clause for admin user IDs
        $adminIdList = $adminUserIds -join ','

        # Get count of tickets submitted by admins
        $countCmd = $connection.CreateCommand()
        $countCmd.CommandText = "SELECT COUNT(*) FROM Tickets WHERE SubmittedBy IN ($adminIdList)"
        $ticketCount = [int]$countCmd.ExecuteScalar()
        
        Write-Host "Found $ticketCount ticket(s) submitted by administrators" -ForegroundColor Yellow

        if ($ticketCount -eq 0) {
            Write-Host "No admin tickets to delete!" -ForegroundColor Green
            Write-Host ""
            return
        }

        # Get the ticket IDs and details to delete
        $getTicketsCmd = $connection.CreateCommand()
        $getTicketsCmd.CommandText = @"
SELECT t.TicketID, t.Title, t.Status, u.Username 
FROM Tickets t 
INNER JOIN Users u ON t.SubmittedBy = u.UserID 
WHERE t.SubmittedBy IN ($adminIdList)
ORDER BY t.TicketID
"@
        $ticketReader = $getTicketsCmd.ExecuteReader()
        
        $ticketIdsToDelete = @()
        Write-Host ""
        Write-Host "Tickets to be deleted:" -ForegroundColor Cyan
        while ($ticketReader.Read()) {
            $ticketId = $ticketReader["TicketID"]
            $title = $ticketReader["Title"]
            $status = $ticketReader["Status"]
            $username = $ticketReader["Username"]
            $ticketIdsToDelete += $ticketId
            Write-Host "  #$ticketId - $title [$status] (by @$username)" -ForegroundColor Gray
        }
        $ticketReader.Close()

        Write-Host ""
        Write-Host "Deleting related records..." -ForegroundColor Cyan

        $totalUpdatesDeleted = 0
        $totalAttachmentsDeleted = 0

        # Delete related TicketUpdates first
        foreach ($ticketId in $ticketIdsToDelete) {
            $deleteUpdatesCmd = $connection.CreateCommand()
            $deleteUpdatesCmd.CommandText = "DELETE FROM TicketUpdates WHERE TicketID = ?"
            $deleteUpdatesCmd.Parameters.AddWithValue("@TicketID", $ticketId) | Out-Null
            $updatesDeleted = $deleteUpdatesCmd.ExecuteNonQuery()
            $totalUpdatesDeleted += $updatesDeleted
            if ($updatesDeleted -gt 0) {
                Write-Host "  Deleted $updatesDeleted update(s) for Ticket #$ticketId" -ForegroundColor DarkGray
            }
        }

        # Delete related Attachments
        foreach ($ticketId in $ticketIdsToDelete) {
            $deleteAttachmentsCmd = $connection.CreateCommand()
            $deleteAttachmentsCmd.CommandText = "DELETE FROM Attachments WHERE TicketID = ?"
            $deleteAttachmentsCmd.Parameters.AddWithValue("@TicketID", $ticketId) | Out-Null
            $attachmentsDeleted = $deleteAttachmentsCmd.ExecuteNonQuery()
            $totalAttachmentsDeleted += $attachmentsDeleted
            if ($attachmentsDeleted -gt 0) {
                Write-Host "  Deleted $attachmentsDeleted attachment(s) for Ticket #$ticketId" -ForegroundColor DarkGray
            }
        }

        # Delete the tickets
        Write-Host ""
        Write-Host "Deleting tickets..." -ForegroundColor Cyan
        $deleteTicketsCmd = $connection.CreateCommand()
        $deleteTicketsCmd.CommandText = "DELETE FROM Tickets WHERE SubmittedBy IN ($adminIdList)"
        $ticketsDeleted = $deleteTicketsCmd.ExecuteNonQuery()

        Write-Host ""
        Write-Host "Cleanup Complete for $dbName!" -ForegroundColor Green
        Write-Host "  Tickets deleted:      $ticketsDeleted" -ForegroundColor Green
        Write-Host "  Updates deleted:      $totalUpdatesDeleted" -ForegroundColor Green
        Write-Host "  Attachments deleted:  $totalAttachmentsDeleted" -ForegroundColor Green
        Write-Host ""

        # Show remaining ticket count
        $remainingCmd = $connection.CreateCommand()
        $remainingCmd.CommandText = "SELECT COUNT(*) FROM Tickets"
        $remainingCount = [int]$remainingCmd.ExecuteScalar()
        Write-Host "  Remaining tickets: $remainingCount" -ForegroundColor Cyan

        # Show breakdown by user role if tickets remain
        if ($remainingCount -gt 0) {
            Write-Host ""
            Write-Host "Remaining tickets by submitter role:" -ForegroundColor Cyan
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
                Write-Host "  $role : $count ticket(s)" -ForegroundColor Gray
            }
            $breakdownReader.Close()
        }

        Write-Host ""

    } catch {
        Write-Host ""
        Write-Host "Error processing $dbName : $($_.Exception.Message)" -ForegroundColor Red
        Write-Host ""
        if ($_.Exception.Message -like "*database is locked*") {
            Write-Host "The database is currently locked. Please:" -ForegroundColor Yellow
            Write-Host "  1. Close the IT Helpdesk application" -ForegroundColor Yellow
            Write-Host "  2. Run this script again" -ForegroundColor Yellow
        }
    } finally {
        if ($connection.State -eq 'Open') {
            $connection.Close()
        }
    }
}

# Clean each database
foreach ($db in $databasesToClean) {
    Clean-Database -dbPath $db.Path -dbName $db.Name
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "All Databases Processed!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host ""
Write-Host "IMPORTANT: Restart the application to see the changes!" -ForegroundColor Yellow
Write-Host ""
