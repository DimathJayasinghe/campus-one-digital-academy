#!/usr/bin/env bash
# ---------------------------------------------------------------
# db-start.sh  –  Start SQL Server container + apply init schema
# ---------------------------------------------------------------
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
COMPOSE_FILE="$SCRIPT_DIR/../docker/docker-compose.yml"

echo "▶  Starting SQL Server 2022 container..."
docker compose -f "$COMPOSE_FILE" up -d

echo "⏳  Waiting for SQL Server to be healthy..."
# Check for sqlcmd location inside container
SQLCMD="/opt/mssql-tools18/bin/sqlcmd"
if ! docker compose -f "$COMPOSE_FILE" exec -T sqlserver test -f "$SQLCMD" 2>/dev/null; then
    SQLCMD="/opt/mssql-tools/bin/sqlcmd"
fi

until docker compose -f "$COMPOSE_FILE" exec -T sqlserver \
      "$SQLCMD" -S localhost -U sa -P "CampusOne@2024" \
      -Q "SELECT 1" -C -N -b > /dev/null 2>&1; do
    printf "."
    sleep 3
done
echo ""
echo "✅  SQL Server is ready."

echo "▶  Applying init.sql schema..."
docker compose -f "$COMPOSE_FILE" exec -T sqlserver \
    "$SQLCMD" \
    -S localhost -U sa -P "CampusOne@2024" \
    -i /docker-entrypoint-initdb.d/init.sql -C -N

echo "✅  Database initialised successfully."
echo "----------------------------------------------------------------------"
echo "  Server:   localhost,1433"
echo "  Database: Student"
echo "  User:     sa"
echo "  Password: CampusOne@2024"
echo "  Connection String: Server=localhost,1433;Database=Student;User Id=sa;Password=CampusOne@2024;TrustServerCertificate=True;"
echo "----------------------------------------------------------------------"
