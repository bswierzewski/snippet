<#
.SYNOPSIS
    Generates .env.example files from IOptions implementations.

.DESCRIPTION
    Uses BuildingBlocks.Tools to scan assemblies and generate .env.example files
    for both the main application (Snippet.Web) and end-to-end tests.

.EXAMPLE
    ./scripts/generate-env.ps1

.EXAMPLE
    ./scripts/generate-env.ps1 -BuildFirst:$false
#>

param(
    [switch]$BuildFirst = $true
)

$ErrorActionPreference = "Stop"
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$backendRoot = Split-Path -Parent $scriptPath
$buildingBlocksRoot = Join-Path $backendRoot "libs/BuildingBlocks"

Write-Host "=== Generating .env.example files ===" -ForegroundColor Cyan

# Build projects if needed
if ($BuildFirst) {
    Write-Host "`nBuilding required projects..." -ForegroundColor Yellow

    Push-Location $backendRoot
    try {
        # Build Tools
        Write-Host "  Building BuildingBlocks.Tools..."
        dotnet build "libs/BuildingBlocks/tools/BuildingBlocks.Tools" --verbosity quiet
        if ($LASTEXITCODE -ne 0) { throw "Failed to build BuildingBlocks.Tools" }

        # Build Snippet.Tests.EndToEnd (includes all dependencies)
        Write-Host "  Building Snippet.Tests.EndToEnd..."
        dotnet build "tests/Snippet.Tests.EndToEnd" --verbosity quiet
        if ($LASTEXITCODE -ne 0) { throw "Failed to build Snippet.Tests.EndToEnd" }
    }
    finally {
        Pop-Location
    }
}

# Define generation tasks
$tasks = @(
    @{
        Name = "Snippet.Web"
        Assemblies = @(
            # Scan main web assembly - has all module references
            "src/Host/Snippet.Web/bin/Debug/net9.0/Snippet.Web.dll"
        )
        Output = "src/Host/Snippet.Web/.env.example"
    },
    @{
        Name = "Snippet.Tests.EndToEnd"
        Assemblies = @(
            # Scan test assembly - has all test and module references
            "tests/Snippet.Tests.EndToEnd/bin/Debug/net9.0/Snippet.Tests.EndToEnd.dll"
        )
        Output = "tests/Snippet.Tests.EndToEnd/.env.example"
    }
)

# Generate .env files
Push-Location $buildingBlocksRoot
try {
    foreach ($task in $tasks) {
        Write-Host "`nGenerating $($task.Name)..." -ForegroundColor Green

        $args = @("run", "--project", "tools/BuildingBlocks.Tools", "--", "env", "generate")

        foreach ($assembly in $task.Assemblies) {
            $fullPath = Join-Path $backendRoot $assembly
            if (-not (Test-Path $fullPath)) {
                Write-Warning "Assembly not found: $assembly"
                continue
            }
            $args += "-a"
            $args += $fullPath
        }

        $outputPath = Join-Path $backendRoot $task.Output
        $args += "-o"
        $args += $outputPath
        $args += "-f"

        & dotnet @args

        if ($LASTEXITCODE -eq 0) {
            Write-Host "  Generated: $($task.Output)" -ForegroundColor Green
        } else {
            Write-Warning "  Failed to generate: $($task.Output)"
        }
    }
}
finally {
    Pop-Location
}

Write-Host "`n=== Done ===" -ForegroundColor Cyan
