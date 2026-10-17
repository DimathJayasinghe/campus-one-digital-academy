# Campus One Digital Academy

> Student Management System — L3 Diploma in Information Technology Final Project  
> ESOFT Metro Campus · Campus One Digital Academy

[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)](https://hub.docker.com/_/microsoft-mssql-server)
[![Docker](https://img.shields.io/badge/Docker-required-blue)](https://www.docker.com)

---

## What This Is

A Windows Forms desktop application that lets an admin log in and manage student registrations (Create, Read, Update, Delete).

| Form | Purpose |
|---|---|
| **Login** | Authenticate admin user |
| **Registration** | Full CRUD for student records |

---

## Quick Start

### Prerequisites
- .NET SDK 10.0+
- Docker Desktop

### 1. Start the Database

```bash
bash scripts/db-start.sh
```

This spins up SQL Server 2022 in Docker and seeds the `Student` database.

### 2. Run the App (Windows only — WinForms)

```bash
dotnet run --project src/CampusOne.App
```

**Login:** `Admin` / `Campusone@123`

---

## Project Structure

```
src/
  CampusOne.Core/       Domain models & interfaces
  CampusOne.Data/       ADO.NET repositories
  CampusOne.App/        WinForms UI
docker/
  docker-compose.yml    SQL Server 2022 container
scripts/
  init.sql              Database + table creation
  db-start.sh           One-command DB startup
docs/
  ARCHITECTURE.md
  DATABASE_SCHEMA.md
  DESIGN_PATTERNS.md
  DEVELOPER_GUIDE.md
```

---

## Architecture

Three-layer architecture using **Repository Pattern** and **Dependency Inversion**:

```
WinForms UI (App) → Interfaces (Core) ← ADO.NET (Data) → SQL Server (Docker)
```

See [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) for the full diagram.

---

## Database

SQL Server 2022 via Docker. Two tables:

- **`Logins`** — application users (default: Admin/Campusone@123)
- **`Registration`** — student records (12 columns, `regNo` as PK)

See [`docs/DATABASE_SCHEMA.md`](docs/DATABASE_SCHEMA.md) for the full schema.

---

## Design Patterns

| Pattern | Purpose |
|---|---|
| Repository | Isolates SQL from UI |
| Factory Method | Creates SQL connections |
| Composition Root | Wires dependencies in `Program.cs` |
| Dependency Inversion | Forms depend on interfaces |
| Observer (Events) | Decoupled form-to-form communication |

See [`docs/DESIGN_PATTERNS.md`](docs/DESIGN_PATTERNS.md) for code examples.

---

## Development

See [`docs/DEVELOPER_GUIDE.md`](docs/DEVELOPER_GUIDE.md) for:
- Adding new features
- Git workflow
- Docker commands
- Troubleshooting

---

## Author

Anuji Galappaththi — L3 Diploma in Information Technology, ESOFT Metro Campus
