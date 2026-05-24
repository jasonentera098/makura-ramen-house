# DeleteInvalidTicketsFromRuntime.ps1
# Deletes tickets without valid SubmittedBy from RUNTIME database

$dbPath = "bin\Debug\net8.0-windows\Data\HelpdeskDB.accdb"

if (-not (Test-Path $dbPath)) {
    Write-Host "Runtime database not found at: $dbPath" -ForegroundColor Red
    exit 1
}

Write-Host "Target: RUNTIME database at $dbPath" -ForegroundColor Cyan
Write-Host ""
Write-Host "IMPORTANT: Close the IT Helpdesk application before running this script!" -ForegroundColor Yellow
Write-Host "Press any key to continue or Ctrl+C to cancel..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Write-Host ""

$connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath"
$connection = New-Object System.Data.OleDb.OleDbConnection($connectionString)

try {
    $connection.Open()
    Write-Host "Connected to runtime database" -ForegroundColor Green

    # Get count of tickets without valid SubmittedBy
    $countCmd = $connection.CreateCommand()
    $countCmd.CommandText = @"
SELECT COUNT(*) as TicketCount 
FROM Tickets 
WHERE SubmittedBy NOT IN (SELECT UserID FROM Users)
"@
    $ticketCount = [int]$countCmd.ExecuteScalar()
    
    Write-Host "Found $ticketCount tickets without valid SubmittedBy" -ForegroundColor Yellow

    if ($ticketCount -eq 0) {
        Write-Host "No invalid tickets to clean up!" -ForegroundColor Green
        exit 0
    }

    # Get the ticket IDs to delete
    $getTicketIdsCmd = $connection.CreateCommand()
    $getTicketIdsCmd.CommandText = @"
SELECT TicketID 
FROM Tickets 
WHERE SubmittedBy NOT IN (SELECT UserID FROM Users)
"@
    $ticketReader = $getTicketIdsCmd.ExecuteReader()
    
    $ticketIdsToDelete = @()
    while ($ticketReader.Read()) {
        $ticketIdsToDelete += $ticketReader["TicketID"]
    }
    $ticketReader.Close()

    Write-Host "Ticket IDs to delete: $($ticketIdsToDelete -join ', ')" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "Deleting related records..." -ForegroundColor Cyan

    # Delete related TicketUpdates first
    foreach ($ticketId in $ticketIdsToDelete) {
        $deleteUpdatesCmd = $connection.CreateCommand()
        $deleteUpdatesCmd.CommandText = "DELETE FROM TicketUpdates WHERE TicketID = ?"
        $deleteUpdatesCmd.Parameters.AddWithValue("@TicketID", $ticketId) | Out-Null
        $updatesDeleted = $deleteUpdatesCmd.ExecuteNonQuery()
        if ($updatesDeleted -gt 0) {
            Write-Host "  Deleted $updatesDeleted update(s) for Ticket #$ticketId" -ForegroundColor Gray
        }
    }

    # Delete related Attachments
    foreach ($ticketId in $ticketIdsToDelete) {
        $deleteAttachmentsCmd = $connection.CreateCommand()
        $deleteAttachmentsCmd.CommandText = "DELETE FROM Attachments WHERE TicketID = ?"
        $deleteAttachmentsCmd.Parameters.AddWithValue("@TicketID", $ticketId) | Out-Null
        $attachmentsDeleted = $deleteAttachmentsCmd.ExecuteNonQuery()
        if ($attachmentsDeleted -gt 0) {
            Write-Host "  Deleted $attachmentsDeleted attachment(s) for Ticket #$ticketId" -ForegroundColor Gray
        }
    }

    # Delete the tickets
    $deleteTicketsCmd = $connection.CreateCommand()
    $deleteTicketsCmd.CommandText = @"
DELETE FROM Tickets 
WHERE SubmittedBy NOT IN (SELECT UserID FROM Users)
"@
    $ticketsDeleted = $deleteTicketsCmd.ExecuteNonQuery()

    Write-Host "`nCleanup complete!" -ForegroundColor Green
    Write-Host "  Invalid tickets deleted: $ticketsDeleted" -ForegroundColor Green

    # Show remaining ticket count
    $remainingCmd = $connection.CreateCommand()
    $remainingCmd.CommandText = "SELECT COUNT(*) FROM Tickets"
    $remainingCount = [int]$remainingCmd.ExecuteScalar()
    Write-Host "  Remaining tickets: $remainingCount" -ForegroundColor Cyan

    Write-Host "`nIMPORTANT: Restart the application to see the changes!" -ForegroundColor Yellow

} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`nNote: If you see 'database is locked' error, close the application first." -ForegroundColor Yellow
    exit 1
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host "`nDone!" -ForegroundColor Green
