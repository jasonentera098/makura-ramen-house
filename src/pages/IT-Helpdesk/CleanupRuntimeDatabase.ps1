# CleanupRuntimeDatabase.ps1
# Deletes ALL tickets from the RUNTIME database (bin\Debug\net8.0-windows\Data\HelpdeskDB.accdb)

$dbPath = "bin\Debug\net8.0-windows\Data\HelpdeskDB.accdb"

if (-not (Test-Path $dbPath)) {
    Write-Host "Runtime database not found at: $dbPath" -ForegroundColor Red
    Write-Host "The application may not have been built yet." -ForegroundColor Yellow
    exit 1
}

Write-Host "Target database: $dbPath" -ForegroundColor Cyan
Write-Host "WARNING: This will delete ALL tickets from the RUNTIME database!" -ForegroundColor Yellow
Write-Host "Press any key to continue or Ctrl+C to cancel..." -ForegroundColor Yellow
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
Write-Host ""

$connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath"
$connection = New-Object System.Data.OleDb.OleDbConnection($connectionString)

try {
    $connection.Open()
    Write-Host "Connected to runtime database" -ForegroundColor Green

    # Get current ticket count
    $countCmd = $connection.CreateCommand()
    $countCmd.CommandText = "SELECT COUNT(*) FROM Tickets"
    $ticketCount = [int]$countCmd.ExecuteScalar()
    
    Write-Host "Found $ticketCount tickets to delete" -ForegroundColor Cyan

    if ($ticketCount -eq 0) {
        Write-Host "No tickets to delete!" -ForegroundColor Green
        exit 0
    }

    # Delete all TicketUpdates first (foreign key constraint)
    Write-Host "Deleting ticket updates..." -ForegroundColor Cyan
    $deleteUpdatesCmd = $connection.CreateCommand()
    $deleteUpdatesCmd.CommandText = "DELETE FROM TicketUpdates"
    $updatesDeleted = $deleteUpdatesCmd.ExecuteNonQuery()
    Write-Host "  Deleted $updatesDeleted ticket update(s)" -ForegroundColor Gray

    # Delete all Attachments
    Write-Host "Deleting attachments..." -ForegroundColor Cyan
    $deleteAttachmentsCmd = $connection.CreateCommand()
    $deleteAttachmentsCmd.CommandText = "DELETE FROM Attachments"
    $attachmentsDeleted = $deleteAttachmentsCmd.ExecuteNonQuery()
    Write-Host "  Deleted $attachmentsDeleted attachment(s)" -ForegroundColor Gray

    # Delete all Tickets
    Write-Host "Deleting tickets..." -ForegroundColor Cyan
    $deleteTicketsCmd = $connection.CreateCommand()
    $deleteTicketsCmd.CommandText = "DELETE FROM Tickets"
    $ticketsDeleted = $deleteTicketsCmd.ExecuteNonQuery()
    Write-Host "  Deleted $ticketsDeleted ticket(s)" -ForegroundColor Gray

    Write-Host "`nCleanup complete!" -ForegroundColor Green
    Write-Host "All tickets have been removed from the RUNTIME database." -ForegroundColor Green
    Write-Host "`nIMPORTANT: Restart the application to see the changes!" -ForegroundColor Yellow

} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "`nNote: If the database is locked, close the application first." -ForegroundColor Yellow
    exit 1
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host "`nDone!" -ForegroundColor Green
