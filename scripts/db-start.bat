@echo off
REM ===============================================================
REM db-start.bat  –  Windows Batch script to start SQL Server & seed DB
REM ===============================================================
setlocal EnableDelayedExpansion

set SCRIPT_DIR=%~dp0
set COMPOSE_FILE=%SCRIPT_DIR%..\docker\docker-compose.yml

echo [INFO] Starting SQL Server 2022 container via Docker Compose...
docker compose -f "%COMPOSE_FILE%" up -d
if %ERRORLEVEL% neq 0 (
    echo [ERROR] Failed to start Docker Compose. Ensure Docker Desktop is running.
    exit /b %ERRORLEVEL%
)

echo [INFO] Waiting for SQL Server container to become ready...
:WAIT_LOOP
timeout /t 3 /nobreak > nul
docker compose -f "%COMPOSE_FILE%" exec -T sqlserver sh -c "which /opt/mssql-tools18/bin/sqlcmd >/dev/null 2>&1 && /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'CampusOne@2024' -Q 'SELECT 1' -C -b >/dev/null 2>&1 || /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'CampusOne@2024' -Q 'SELECT 1' -b >/dev/null 2>&1"
if %ERRORLEVEL% neq 0 (
    <nul set /p =.
    goto WAIT_LOOP
)

echo.
echo [INFO] SQL Server is ready.
echo [INFO] Applying init.sql schema...

docker compose -f "%COMPOSE_FILE%" exec -T sqlserver sh -c "if [ -f /opt/mssql-tools18/bin/sqlcmd ]; then /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P 'CampusOne@2024' -i /docker-entrypoint-initdb.d/init.sql -C -N; else /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'CampusOne@2024' -i /docker-entrypoint-initdb.d/init.sql; fi"

echo.
echo [SUCCESS] Database 'Student' initialized successfully!
echo -------------------------------------------------------------
echo   Server:   localhost,1433
echo   Database: Student
echo   User:     sa
echo   Password: CampusOne@2024
echo -------------------------------------------------------------
pause
