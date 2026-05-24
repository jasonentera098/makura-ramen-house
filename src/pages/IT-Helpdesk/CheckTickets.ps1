# CheckTickets.ps1
# Shows all tickets in the database with their SubmittedBy information

$dbPath = "Data\HelpdeskDB.accdb"

if (-not (Test-Path $dbPath)) {
    Write-Host "Database not found at: $dbPath" -ForegroundColor Red
    exit 1
}

$connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath"
$connection = New-Object System.Data.OleDb.OleDbConnection($connectionString)

try {
    $connection.Open()
    Write-Host "Connected to database" -ForegroundColor Green
    Write-Host ""

    # Get all tickets with user information
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = @"
SELECT 
    t.TicketID, 
    t.Title, 
    t.Status, 
    t.Priority,
    t.SubmittedBy,
    u.FullName as SubmittedByName,
    u.Username,
    t.DateSubmitted
FROM Tickets t
LEFT JOIN Users u ON t.SubmittedBy = u.UserID
ORDER BY t.TicketID
"@
    
    $reader = $cmd.ExecuteReader()
    
    $ticketCount = 0
    Write-Host "Current Tickets in Database:" -ForegroundColor Cyan
    Write-Host "=" * 100 -ForegroundColor Gray
    Write-Host ("{0,-5} {1,-30} {2,-15} {3,-10} {4,-20} {5,-15}" -f "ID", "Title", "Status", "Priority", "Submitted By", "Username") -ForegroundColor Yellow
    Write-Host "=" * 100 -ForegroundColor Gray
    
    while ($reader.Read()) {
        $ticketCount++
        $id = $reader["TicketID"]
        $title = $reader["Title"]
        $status = $reader["Status"]
        $priority = $reader["Priority"]
        $submittedBy = if ($reader["SubmittedByName"] -is [DBNull]) { "INVALID USER" } else { $reader["SubmittedByName"] }
        $username = if ($reader["Username"] -is [DBNull]) { "N/A" } else { $reader["Username"] }
        
        $color = if ($submittedBy -eq "INVALID USER") { "Red" } else { "White" }
        
        Write-Host ("{0,-5} {1,-30} {2,-15} {3,-10} {4,-20} {5,-15}" -f $id, $title, $status, $priority, $submittedBy, $username) -ForegroundColor $color
    }
    
    $reader.Close()
    
    Write-Host "=" * 100 -ForegroundColor Gray
    Write-Host "Total tickets: $ticketCount" -ForegroundColor Cyan
    Write-Host ""

} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}
