#!/usr/bin/env bash
# ---------------------------------------------------------------
# db-start.sh  –  Start SQL Server container + apply init schema
# ---------------------------------------------------------------
set -euo pipefail

COMPOSE_FILE="$(dirname "$0")/../docker/docker-compose.yml"

echo "▶  Starting SQL Server container..."
docker compose -f "$COMPOSE_FILE" up -d

echo "⏳  Waiting for SQL Server to be healthy..."
until docker compose -f "$COMPOSE_FILE" exec -T sqlserver \
      /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "CampusOne@2024" \
      -Q "SELECT 1" -C -N -b > /dev/null 2>&1; do
    printf "."
    sleep 3
done
echo ""
echo "✅  SQL Server is ready."

echo "▶  Applying init.sql schema..."
docker compose -f "$COMPOSE_FILE" exec -T sqlserver \
    /opt/mssql-tools18/bin/sqlcmd \
    -S localhost -U sa -P "CampusOne@2024" \
    -i /docker-entrypoint-initdb.d/init.sql -C -N

echo "✅  Database initialised. Connection string:"
echo "    Server=localhost,1433;Database=Student;User Id=sa;Password=CampusOne@2024;TrustServerCertificate=True;"
