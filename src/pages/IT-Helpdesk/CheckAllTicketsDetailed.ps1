# CheckAllTicketsDetailed.ps1
# Shows detailed information about all tickets including SubmittedBy validation

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

    # Get all users first
    Write-Host "Valid Users in Database:" -ForegroundColor Cyan
    $getUsersCmd = $connection.CreateCommand()
    $getUsersCmd.CommandText = "SELECT UserID, FullName, Username, Role FROM Users ORDER BY UserID"
    $userReader = $getUsersCmd.ExecuteReader()
    
    $validUserIds = @()
    while ($userReader.Read()) {
        $userId = $userReader["UserID"]
        $fullName = $userReader["FullName"]
        $username = $userReader["Username"]
        $role = $userReader["Role"]
        $validUserIds += $userId
        Write-Host "  UserID: $userId - $fullName ($username) - $role" -ForegroundColor White
    }
    $userReader.Close()
    Write-Host ""

    # Get all tickets
    Write-Host "All Tickets in Database:" -ForegroundColor Cyan
    $getTicketsCmd = $connection.CreateCommand()
    $getTicketsCmd.CommandText = "SELECT TicketID, Title, Status, Priority, SubmittedBy, AssignedTo, DateSubmitted FROM Tickets ORDER BY TicketID"
    $ticketReader = $getTicketsCmd.ExecuteReader()
    
    $ticketCount = 0
    $invalidTickets = @()
    
    while ($ticketReader.Read()) {
        $ticketCount++
        $ticketId = $ticketReader["TicketID"]
        $title = $ticketReader["Title"]
        $status = $ticketReader["Status"]
        $priority = $ticketReader["Priority"]
        $submittedBy = $ticketReader["SubmittedBy"]
        $assignedTo = if ($ticketReader["AssignedTo"] -is [DBNull]) { "NULL" } else { $ticketReader["AssignedTo"] }
        $dateSubmitted = $ticketReader["DateSubmitted"]
        
        $isValid = $validUserIds -contains $submittedBy
        
        if (-not $isValid) {
            $invalidTickets += $ticketId
        }
        
        $color = if ($isValid) { "White" } else { "Red" }
        $validText = if ($isValid) { "VALID" } else { "INVALID - SubmittedBy user does not exist!" }
        
        Write-Host "  Ticket #$ticketId - $title" -ForegroundColor $color
        Write-Host "    Status: $status | Priority: $priority | SubmittedBy: $submittedBy | AssignedTo: $assignedTo" -ForegroundColor $color
        Write-Host "    Date: $dateSubmitted | Validation: $validText" -ForegroundColor $color
        Write-Host ""
    }
    
    $ticketReader.Close()
    
    Write-Host "=" * 80 -ForegroundColor Gray
    Write-Host "Total tickets: $ticketCount" -ForegroundColor Cyan
    Write-Host "Invalid tickets (no valid SubmittedBy): $($invalidTickets.Count)" -ForegroundColor $(if ($invalidTickets.Count -gt 0) { "Red" } else { "Green" })
    
    if ($invalidTickets.Count -gt 0) {
        Write-Host "Invalid ticket IDs: $($invalidTickets -join ', ')" -ForegroundColor Red
    }

} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    exit 1
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}
