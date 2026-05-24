# PowerShell script to create a default Administrator account directly
# Username: Administrator
# Password: Admin123
# This script assumes the database and tables already exist

$dbPath = ".\Data\HelpdeskDB.accdb"

if (-not (Test-Path $dbPath)) {
    Write-Host "Error: Database file not found at $dbPath" -ForegroundColor Red
    Write-Host "Please run the application first to initialize the database." -ForegroundColor Yellow
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

    # Hash the password: ITHelpdesk_Admin123_Salt2024
    $saltedPassword = "ITHelpdesk_Admin123_Salt2024"
    $sha256 = [System.Security.Cryptography.SHA256]::Create()
    $bytes = [System.Text.Encoding]::UTF8.GetBytes($saltedPassword)
    $hash = $sha256.ComputeHash($bytes)
    $passwordHash = [Convert]::ToBase64String($hash)
    
    Write-Host "Password hash generated: $passwordHash" -ForegroundColor Cyan

    # Check if Administrator user already exists
    $checkCmd = $connection.CreateCommand()
    $checkCmd.CommandText = "SELECT UserID, AccountStatus FROM Users WHERE Username = 'Administrator'"
    
    $reader = $checkCmd.ExecuteReader()
    $userExists = $reader.Read()
    
    if ($userExists) {
        $userId = $reader["UserID"]
        $currentStatus = $reader["AccountStatus"]
        $reader.Close()
        
        Write-Host "Administrator account found (UserID: $userId, Status: $currentStatus)" -ForegroundColor Yellow
        Write-Host "Updating password and setting status to Active..." -ForegroundColor Yellow
        
        # Update existing user
        $updateCmd = $connection.CreateCommand()
        $updateCmd.CommandText = "UPDATE Users SET PasswordHash = ?, AccountStatus = ? WHERE Username = ?"
        $updateCmd.Parameters.Add("@PasswordHash", [System.Data.OleDb.OleDbType]::VarWChar).Value = $passwordHash
        $updateCmd.Parameters.Add("@AccountStatus", [System.Data.OleDb.OleDbType]::VarWChar).Value = "Active"
        $updateCmd.Parameters.Add("@Username", [System.Data.OleDb.OleDbType]::VarWChar).Value = "Administrator"
        
        $rowsAffected = $updateCmd.ExecuteNonQuery()
        Write-Host "Administrator account updated successfully! ($rowsAffected row(s) affected)" -ForegroundColor Green
    }
    else {
        $reader.Close()
        Write-Host "Administrator account not found. Creating new account..." -ForegroundColor Yellow
        
        # Get IT Department ID
        $deptCmd = $connection.CreateCommand()
        $deptCmd.CommandText = "SELECT DepartmentID FROM Departments WHERE DepartmentName = 'IT Department'"
        
        $deptId = $deptCmd.ExecuteScalar()
        
        if ($null -eq $deptId) {
            Write-Host "Error: IT Department not found in database." -ForegroundColor Red
            Write-Host "Please run the application first to initialize departments." -ForegroundColor Yellow
            $connection.Close()
            exit 1
        }
        
        Write-Host "Found IT Department (ID: $deptId)" -ForegroundColor Cyan
        
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
        
        $rowsAffected = $insertCmd.ExecuteNonQuery()
        Write-Host "Administrator account created successfully! ($rowsAffected row(s) affected)" -ForegroundColor Green
    }
    
    Write-Host ""
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host "  Login Credentials:" -ForegroundColor Cyan
    Write-Host "  Username: Administrator" -ForegroundColor White
    Write-Host "  Password: Admin123" -ForegroundColor White
    Write-Host "========================================" -ForegroundColor Cyan
    Write-Host ""
    
    $connection.Close()
}
catch {
    Write-Host ""
    Write-Host "Error: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host ""
    Write-Host "Troubleshooting:" -ForegroundColor Yellow
    Write-Host "1. Make sure the application has been run at least once to create database tables" -ForegroundColor Yellow
    Write-Host "2. Close the application if it's currently running" -ForegroundColor Yellow
    Write-Host "3. Ensure Microsoft Access Database Engine is installed" -ForegroundColor Yellow
    Write-Host ""
    
    if ($connection -and $connection.State -eq 'Open') {
        $connection.Close()
    }
    exit 1
}
