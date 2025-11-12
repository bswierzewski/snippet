#!/usr/bin/env pwsh
# Script to drop the database for Snippet application

Write-Host "Dropping Snippet database..." -ForegroundColor Yellow

# Navigate to backend directory
$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendDir = Split-Path -Parent $scriptDir

Push-Location $backendDir

try {
    # Drop the database using Entity Framework
    dotnet ef database drop `
        --project src/Modules/Snippets/Snippet.Modules.Snippets.Infrastructure `
        --startup-project src/Host/Snippet.Web `
        --context SnippetsDbContext `
        --force

    if ($LASTEXITCODE -eq 0) {
        Write-Host "Database dropped successfully!" -ForegroundColor Green
    } else {
        Write-Host "Failed to drop database!" -ForegroundColor Red
        exit 1
    }
} finally {
    Pop-Location
}
