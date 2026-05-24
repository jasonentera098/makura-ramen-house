# UpdateFontsToRoboto.ps1
# Replaces all "Segoe UI" font references with "Roboto" in XAML files

Write-Host "Updating all fonts to Roboto..." -ForegroundColor Cyan
Write-Host ""

$xamlFiles = Get-ChildItem -Path . -Filter "*.xaml" -Recurse | Where-Object { $_.FullName -notlike "*\obj\*" -and $_.FullName -notlike "*\bin\*" }

$totalFiles = 0
$totalReplacements = 0

foreach ($file in $xamlFiles) {
    $content = Get-Content $file.FullName -Raw
    $originalContent = $content
    
    # Replace all instances of Segoe UI with Roboto
    $content = $content -replace 'FontFamily="Segoe UI"', 'FontFamily="Roboto"'
    
    if ($content -ne $originalContent) {
        $replacements = ([regex]::Matches($originalContent, 'FontFamily="Segoe UI"')).Count
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "[OK] Updated: $($file.Name) - $replacements replacements" -ForegroundColor Green
        $totalFiles++
        $totalReplacements += $replacements
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Green
Write-Host "Font Update Complete!" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Green
Write-Host "Files updated: $totalFiles" -ForegroundColor Cyan
Write-Host "Total replacements: $totalReplacements" -ForegroundColor Cyan
Write-Host ""
Write-Host "All fonts have been changed to Roboto!" -ForegroundColor Green
Write-Host ""
