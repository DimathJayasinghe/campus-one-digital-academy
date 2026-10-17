# ===============================================================
# db-start.ps1  –  PowerShell script to start SQL Server & seed DB
# ===============================================================
$ErrorActionPreference = "Stop"

$scriptDir = Split-Path -Parent $MyInvocation.MyCommand.Path
$composeFile = Join-Path $scriptDir "..\docker\docker-compose.yml"

Write-Host "▶  Starting SQL Server 2022 container via Docker Compose..." -ForegroundColor Cyan
docker compose -f $composeFile up -d

Write-Host "⏳  Waiting for SQL Server container to become ready..." -ForegroundColor Yellow
$ready = $false
$attempts = 0
$maxAttempts = 30

while (-not $ready -and $attempts -lt $maxAttempts) {
    Start-Sleep -Seconds 3
    $attempts++
    try {
        $check = docker compose -f $composeFile exec -T sqlserver sh -c "
            if [ -f /opt/mssql-tools18/bin/sqlcmd ]; then
                /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'CampusOne@2024' -Q 'SELECT 1' -C -b > /dev/null 2>&1
            else
                /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'CampusOne@2024' -Q 'SELECT 1' -b > /dev/null 2>&1
            fi
        "
        if ($LASTEXITCODE -eq 0) {
            $ready = $true
        } else {
            Write-Host -NoNewline "."
        }
    } catch {
        Write-Host -NoNewline "."
    }
}

if (-not $ready) {
    Write-Host "`n❌ Timed out waiting for SQL Server." -ForegroundColor Red
    exit 1
}

Write-Host "`n✅  SQL Server is up and ready." -ForegroundColor Green
Write-Host "▶  Applying init.sql schema..." -ForegroundColor Cyan

docker compose -f $composeFile exec -T sqlserver sh -c "
    if [ -f /opt/mssql-tools18/bin/sqlcmd ]; then
        /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'CampusOne@2024' -i /docker-entrypoint-initdb.d/init.sql -C -N
    else
        /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'CampusOne@2024' -i /docker-entrypoint-initdb.d/init.sql
    fi
"

Write-Host "✅  Database initialized successfully!" -ForegroundColor Green
Write-Host "------------------------------------------------------------------" -ForegroundColor Gray
Write-Host "  Database: Student"
Write-Host "  Server:   localhost,1433"
Write-Host "  SA User:  sa / CampusOne@2024"
Write-Host "------------------------------------------------------------------" -ForegroundColor Gray
