# Complete Database Initialization Script
$dbPath = ".\Data\HelpdeskDB.accdb"
$connectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$dbPath;Persist Security Info=False;"

Write-Host "Initializing database..." -ForegroundColor Cyan
Write-Host ""

Add-Type -AssemblyName System.Data
$connection = New-Object System.Data.OleDb.OleDbConnection($connectionString)
$connection.Open()
Write-Host "Connected to database." -ForegroundColor Green

# Create Departments table
try {
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = 'CREATE TABLE Departments (DepartmentID AUTOINCREMENT PRIMARY KEY, DepartmentName TEXT(100) NOT NULL, Description TEXT(255))'
    $cmd.ExecuteNonQuery() | Out-Null
    Write-Host "Created Departments table" -ForegroundColor Green
} catch { Write-Host "Departments table exists" -ForegroundColor Yellow }

# Create Users table
try {
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = 'CREATE TABLE Users (UserID AUTOINCREMENT PRIMARY KEY, FullName TEXT(100) NOT NULL, Username TEXT(50) NOT NULL, PasswordHash TEXT(255) NOT NULL, Role TEXT(20) NOT NULL, DepartmentID INTEGER NOT NULL, ContactNumber TEXT(20), AccountStatus TEXT(20) DEFAULT ''Active'', CreatedAt DATETIME NOT NULL)'
    $cmd.ExecuteNonQuery() | Out-Null
    Write-Host "Created Users table" -ForegroundColor Green
} catch { Write-Host "Users table exists" -ForegroundColor Yellow }

# Seed departments
$checkCmd = $connection.CreateCommand()
$checkCmd.CommandText = 'SELECT COUNT(*) FROM Departments'
$count = [int]$checkCmd.ExecuteScalar()

if ($count -eq 0) {
    Write-Host "Seeding departments..." -ForegroundColor Cyan
    $depts = @(
        @('IT Department', 'Provides technical support'),
        @('Human Resources', 'Manages employee relations'),
        @('Accounting', 'Handles financial records'),
        @('Sales', 'Manages client relationships'),
        @('Operations', 'Oversees operations'),
        @('Registrar', 'Manages records')
    )
    foreach ($d in $depts) {
        $cmd = $connection.CreateCommand()
        $cmd.CommandText = 'INSERT INTO Departments (DepartmentName, Description) VALUES (?, ?)'
        $p1 = New-Object System.Data.OleDb.OleDbParameter('p1', [System.Data.OleDb.OleDbType]::VarWChar)
        $p1.Value = $d[0]
        $p2 = New-Object System.Data.OleDb.OleDbParameter('p2', [System.Data.OleDb.OleDbType]::VarWChar)
        $p2.Value = $d[1]
        $cmd.Parameters.Add($p1) | Out-Null
        $cmd.Parameters.Add($p2) | Out-Null
        $cmd.ExecuteNonQuery() | Out-Null
    }
    Write-Host "Departments seeded" -ForegroundColor Green
}

# Get IT Department ID
$deptCmd = $connection.CreateCommand()
$deptCmd.CommandText = 'SELECT DepartmentID FROM Departments WHERE DepartmentName = ''IT Department'''
$itDeptId = $deptCmd.ExecuteScalar()

# Create Administrator
$checkUserCmd = $connection.CreateCommand()
$checkUserCmd.CommandText = 'SELECT COUNT(*) FROM Users WHERE Username = ''Administrator'''
$userCount = [int]$checkUserCmd.ExecuteScalar()

$saltedPassword = 'ITHelpdesk_Admin123_Salt2024'
$sha256 = [System.Security.Cryptography.SHA256]::Create()
$bytes = [System.Text.Encoding]::UTF8.GetBytes($saltedPassword)
$hash = $sha256.ComputeHash($bytes)
$passwordHash = [Convert]::ToBase64String($hash)

if ($userCount -eq 0) {
    Write-Host "Creating Administrator account..." -ForegroundColor Cyan
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = 'INSERT INTO Users (FullName, Username, PasswordHash, Role, DepartmentID, ContactNumber, AccountStatus, CreatedAt) VALUES (?, ?, ?, ?, ?, ?, ?, ?)'
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p1', [System.Data.OleDb.OleDbType]::VarWChar))).Value = 'System Administrator'
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p2', [System.Data.OleDb.OleDbType]::VarWChar))).Value = 'Administrator'
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p3', [System.Data.OleDb.OleDbType]::VarWChar))).Value = $passwordHash
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p4', [System.Data.OleDb.OleDbType]::VarWChar))).Value = 'Administrator'
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p5', [System.Data.OleDb.OleDbType]::Integer))).Value = [int]$itDeptId
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p6', [System.Data.OleDb.OleDbType]::VarWChar))).Value = ''
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p7', [System.Data.OleDb.OleDbType]::VarWChar))).Value = 'Active'
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p8', [System.Data.OleDb.OleDbType]::Date))).Value = [DateTime]::Now
    $cmd.ExecuteNonQuery() | Out-Null
    Write-Host "Administrator created!" -ForegroundColor Green
} else {
    Write-Host "Administrator already exists, updating..." -ForegroundColor Yellow
    $cmd = $connection.CreateCommand()
    $cmd.CommandText = 'UPDATE Users SET PasswordHash = ?, AccountStatus = ? WHERE Username = ?'
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p1', [System.Data.OleDb.OleDbType]::VarWChar))).Value = $passwordHash
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p2', [System.Data.OleDb.OleDbType]::VarWChar))).Value = 'Active'
    $cmd.Parameters.Add((New-Object System.Data.OleDb.OleDbParameter('p3', [System.Data.OleDb.OleDbType]::VarWChar))).Value = 'Administrator'
    $cmd.ExecuteNonQuery() | Out-Null
    Write-Host "Administrator updated!" -ForegroundColor Green
}

$connection.Close()
Write-Host ""
Write-Host "======================================" -ForegroundColor Cyan
Write-Host "  COMPLETE!" -ForegroundColor Green
Write-Host "  Username: Administrator" -ForegroundColor Yellow
Write-Host "  Password: Admin123" -ForegroundColor Yellow
Write-Host "======================================" -ForegroundColor Cyan
