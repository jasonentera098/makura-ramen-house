# CreateDatabase.ps1
# Manual helper script to create the HelpdeskDB.accdb file using COM automation.
# Run this script if the application cannot create the database automatically at startup.
#
# Prerequisites:
#   - Microsoft Access Database Engine 2016 (ACE) must be installed.
#     Download: https://www.microsoft.com/en-us/download/details.aspx?id=54920

param(
    [string]$DbPath = "$PSScriptRoot\Data\HelpdeskDB.accdb"
)

# Ensure the Data directory exists
$dataDir = Split-Path -Parent $DbPath
if (-not (Test-Path $dataDir)) {
    New-Item -ItemType Directory -Path $dataDir | Out-Null
    Write-Host "Created directory: $dataDir"
}

# Create the .accdb file via ADOX if it doesn't already exist
if (-not (Test-Path $DbPath)) {
    Write-Host "Creating database file at: $DbPath"
    try {
        $catalog = New-Object -ComObject "ADOX.Catalog"
        $connStr = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$DbPath;"
        $catalog.Create($connStr)
        [System.Runtime.InteropServices.Marshal]::ReleaseComObject($catalog) | Out-Null
        Write-Host "Database file created successfully."
    }
    catch {
        Write-Error "Failed to create database: $_"
        Write-Error "Ensure the Microsoft Access Database Engine is installed."
        exit 1
    }
}
else {
    Write-Host "Database file already exists: $DbPath"
}

Write-Host "Done. The application will create the tables automatically on first startup."
