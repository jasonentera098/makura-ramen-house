# CheckRuntimeTickets.ps1
# Check tickets in the runtime database

$dbPath = "bin\Debug\net8.0-windows\Data\HelpdeskDB.accdb"

if (-not (Test-Path $dbPath)) {
    Write-Host "Runtime database not found at: $dbPath" -ForegroundColor Red
    exit 1
}

Write-Host "Checking runtime database: $dbPath" -ForegroundColor Cyan
Write-Host ""

$connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath"
$connection = New-Object System.Data.OleDb.OleDbConnection($connectionString)

try {
    $connection.Open()
    Write-Host "Connected successfully!" -ForegroundColor Green
    Write-Host ""

    # Get all tickets with submitter info
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = @"
SELECT t.TicketID, t.Title, t.Status, t.Priority, u.Username, u.FullName, u.Role
FROM Tickets t
INNER JOIN Users u ON t.SubmittedBy = u.UserID
ORDER BY t.TicketID
"@
    
    $reader = $cmd.ExecuteReader()
    
    $tickets = @()
    while ($reader.Read()) {
        $tickets += [PSCustomObject]@{
            ID = $reader["TicketID"]
            Title = $reader["Title"]
            Status = $reader["Status"]
            Priority = $reader["Priority"]
            Username = $reader["Username"]
            FullName = $reader["FullName"]
            Role = $reader["Role"]
        }
    }
    $reader.Close()

    Write-Host "Total Tickets: $($tickets.Count)" -ForegroundColor Yellow
    Write-Host ""

    if ($tickets.Count -gt 0) {
        Write-Host "Tickets in database:" -ForegroundColor Cyan
        Write-Host ("=" * 120)
        Write-Host ("{0,-4} {1,-35} {2,-15} {3,-10} {4,-20} {5,-15}" -f "ID", "Title", "Status", "Priority", "Submitted By", "Role")
        Write-Host ("=" * 120)
        
        foreach ($ticket in $tickets) {
            $color = "White"
            if ($ticket.Role -eq "Administrator") {
                $color = "Yellow"
            }
            Write-Host ("{0,-4} {1,-35} {2,-15} {3,-10} {4,-20} {5,-15}" -f `
                $ticket.ID, `
                $ticket.Title.Substring(0, [Math]::Min(34, $ticket.Title.Length)), `
                $ticket.Status, `
                $ticket.Priority, `
                $ticket.FullName.Substring(0, [Math]::Min(19, $ticket.FullName.Length)), `
                $ticket.Role) -ForegroundColor $color
        }
        Write-Host ("=" * 120)
        Write-Host ""

        # Count by role
        $adminTickets = ($tickets | Where-Object { $_.Role -eq "Administrator" }).Count
        $employeeTickets = ($tickets | Where-Object { $_.Role -eq "Employee" }).Count
        $technicianTickets = ($tickets | Where-Object { $_.Role -eq "Technician" }).Count

        Write-Host "Breakdown by submitter role:" -ForegroundColor Cyan
        if ($adminTickets -gt 0) {
            Write-Host "  Administrator: $adminTickets ticket(s)" -ForegroundColor Yellow
        }
        if ($employeeTickets -gt 0) {
            Write-Host "  Employee: $employeeTickets ticket(s)" -ForegroundColor Green
        }
        if ($technicianTickets -gt 0) {
            Write-Host "  Technician: $technicianTickets ticket(s)" -ForegroundColor Cyan
        }
    }

} catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    if ($connection.State -eq 'Open') {
        $connection.Close()
    }
}

Write-Host ""
