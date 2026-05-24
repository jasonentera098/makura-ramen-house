# PowerShell script to create a default Administrator account
# Username: Administrator
# Password: Admin123

$dbPath = ".\Data\HelpdeskDB.accdb"

if (-not (Test-Path $dbPath)) {
    Write-Host "Error: Database file not found at $dbPath" -ForegroundColor Red
    exit 1
}

$connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath;Persist Security Info=False;"

try {
    # Load the required assembly
    Add-Type -AssemblyName System.Data

    # Create connection
    $connection = New-Object System.Data.OleDb.OleDbConnection($connectionString)
    $connection.Open()
    Write-Host "Connected to database successfully." -ForegroundColor Green

    # Check if Administrator user already exists
    $checkCmd = $connection.CreateCommand()
    $checkCmd.CommandText = "SELECT COUNT(*) FROM Users WHERE Username = ?"
    $checkParam = $checkCmd.Parameters.Add("@Username", [System.Data.OleDb.OleDbType]::VarWChar)
    $checkParam.Value = "Administrator"
    
    $count = [int]$checkCmd.ExecuteScalar()
    
    if ($count -gt 0) {
        Write-Host "Administrator account already exists. Updating password..." -ForegroundColor Yellow
        
        # Hash the password: ITHelpdesk_Admin123_Salt2024
        $saltedPassword = "ITHelpdesk_Admin123_Salt2024"
        $sha256 = [System.Security.Cryptography.SHA256]::Create()
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($saltedPassword)
        $hash = $sha256.ComputeHash($bytes)
        $passwordHash = [Convert]::ToBase64String($hash)
        
        # Update existing user
        $updateCmd = $connection.CreateCommand()
        $updateCmd.CommandText = "UPDATE Users SET PasswordHash = ?, AccountStatus = ? WHERE Username = ?"
        $updateCmd.Parameters.Add("@PasswordHash", [System.Data.OleDb.OleDbType]::VarWChar).Value = $passwordHash
        $updateCmd.Parameters.Add("@AccountStatus", [System.Data.OleDb.OleDbType]::VarWChar).Value = "Active"
        $updateCmd.Parameters.Add("@Username", [System.Data.OleDb.OleDbType]::VarWChar).Value = "Administrator"
        
        $updateCmd.ExecuteNonQuery() | Out-Null
        Write-Host "Administrator account updated successfully!" -ForegroundColor Green
    }
    else {
        Write-Host "Creating new Administrator account..." -ForegroundColor Yellow
        
        # Get IT Department ID
        $deptCmd = $connection.CreateCommand()
        $deptCmd.CommandText = "SELECT DepartmentID FROM Departments WHERE DepartmentName = ?"
        $deptParam = $deptCmd.Parameters.Add("@DepartmentName", [System.Data.OleDb.OleDbType]::VarWChar)
        $deptParam.Value = "IT Department"
        
        $deptId = $deptCmd.ExecuteScalar()
        
        if ($null -eq $deptId) {
            Write-Host "Error: IT Department not found. Please ensure departments are seeded." -ForegroundColor Red
            $connection.Close()
            exit 1
        }
        
        # Hash the password: ITHelpdesk_Admin123_Salt2024
        $saltedPassword = "ITHelpdesk_Admin123_Salt2024"
        $sha256 = [System.Security.Cryptography.SHA256]::Create()
        $bytes = [System.Text.Encoding]::UTF8.GetBytes($saltedPassword)
        $hash = $sha256.ComputeHash($bytes)
        $passwordHash = [Convert]::ToBase64String($hash)
        
        # Insert new admin user
        $insertCmd = $connection.CreateCommand()
        $insertCmd.CommandText = @"
INSERT INTO Users (FullName, Username, PasswordHash, Role, DepartmentID, ContactNumber, AccountStatus, CreatedAt)
VALUES (?, ?, ?, ?, ?, ?, ?, ?)
"@
        
        $insertCmd.Parameters.Add("@FullName", [System.Data.OleDb.OleDbType]::VarWChar).Value = "System Administrator"
        $insertCmd.Parameters.Add("@Username", [System.Data.OleDb.OleDbType]::VarWChar).Value = "Administrator"
        $insertCmd.Parameters.Add("@PasswordHash", [System.Data.OleDb.OleDbType]::VarWChar).Value = $passwordHash
        $insertCmd.Parameters.Add("@Role", [System.Data.OleDb.OleDbType]::VarWChar).Value = "Administrator"
        $insertCmd.Parameters.Add("@DepartmentID", [System.Data.OleDb.OleDbType]::Integer).Value = [int]$deptId
        $insertCmd.Parameters.Add("@ContactNumber", [System.Data.OleDb.OleDbType]::VarWChar).Value = ""
        $insertCmd.Parameters.Add("@AccountStatus", [System.Data.OleDb.OleDbType]::VarWChar).Value = "Active"
        $insertCmd.Parameters.Add("@CreatedAt", [System.Data.OleDb.OleDbType]::Date).Value = [DateTime]::Now
        
        $insertCmd.ExecuteNonQuery() | Out-Null
        Write-Host "Administrator account created successfully!" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "Login Credentials:" -ForegroundColor Cyan
    Write-Host "  Username: Administrator" -ForegroundColor White
    Write-Host "  Password: Admin123" -ForegroundColor White
    Write-Host ""
    
    $connection.Close()
}
catch {
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
    exit 1
}
