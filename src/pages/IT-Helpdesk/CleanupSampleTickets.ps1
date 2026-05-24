# CleanupSampleTickets.ps1
# Removes all tickets that don't have a valid SubmittedBy user

$dbPath = "Data\HelpdeskDB.accdb"

if (-not (Test-Path $dbPath)) {
    Write-Host "Database not found at: $dbPath" -ForegroundColor Red
    exit 1
}

Write-Host "Connecting to database..." -ForegroundColor Cyan

$connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath"
$connection = New-Object System.Data.OleDb.OleDbConnection($connectionString)

try {
    $connection.Open()
    Write-Host "Connected successfully!" -ForegroundColor Green

    # First, get all valid user IDs
    $getUsersCmd = $connection.CreateCommand()
    $getUsersCmd.CommandText = "SELECT UserID FROM Users"
    $userReader = $getUsersCmd.ExecuteReader()
    
    $validUserIds = @()
    while ($userReader.Read()) {
        $validUserIds += $userReader["UserID"]
    }
    $userReader.Close()
    
    Write-Host "Found $($validUserIds.Count) valid users" -ForegroundColor Cyan

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
        Write-Host "No tickets to clean up!" -ForegroundColor Green
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
    Write-Host "  Tickets deleted: $ticketsDeleted" -ForegroundColor Green

    # Show remaining ticket count
    $remainingCmd = $connection.CreateCommand()
    $remainingCmd.CommandText = "SELECT COUNT(*) FROM Tickets"
    $remainingCount = [int]$remainingCmd.ExecuteScalar()
    Write-Host "  Remaining tickets: $remainingCount" -ForegroundColor Cyan

} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host "`nDone!" -ForegroundColor Green
