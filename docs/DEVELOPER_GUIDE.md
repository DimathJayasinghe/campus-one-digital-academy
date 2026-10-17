# Campus One Digital Academy – Developer Guide

---

## Prerequisites

| Tool | Minimum version | Purpose |
|---|---|---|
| .NET SDK | 10.0 | Build & run the app |
| Docker Desktop | 24+ | Run SQL Server 2022 container |
| Visual Studio 2022 | 17.8+ | WinForms designer & execution (Windows) |
| VS Code + C# Dev Kit | Latest | Code editing on Linux/macOS/Windows |
| `gh` CLI (optional) | 2.x | GitHub synchronization |

---

## Quick Start

```bash
# 1. Clone the repo
git clone https://github.com/DimathJayasinghe/campus-one-digital-academy.git
cd campus-one-digital-academy

# 2. Start SQL Server in Docker and initialize database
# Linux / macOS:
bash scripts/db-start.sh

# Windows (PowerShell):
.\scripts\db-start.ps1

# Windows (CMD):
scripts\db-start.bat

# 3. Build and Run (Windows required for WinForms UI runtime)
dotnet run --project src/CampusOne.App
```

**Default Admin Credentials:**
- **Username:** `Admin`
- **Password:** `Campusone@123`

---

## Project Layers – Responsibilities

### `CampusOne.Core` — Domain Layer
- **Contains:** Models (`Student`, `LoginUser`), Interfaces (`IStudentRepository`, `ILoginRepository`)
- **Rule:** Zero external dependencies. No NuGet packages, no SQL syntax, no UI elements.
- **Purpose:** Pure business and domain contracts.

### `CampusOne.Data` — Data Access Layer
- **Contains:** `DbConnectionFactory`, `StudentRepository`, `LoginRepository`
- **Rule:** Only handles database I/O and ADO.NET query execution. No message boxes or UI logic.
- **Purpose:** Safe, parameterized SQL data access with connection pooling.

### `CampusOne.App` — UI Presentation Layer
- **Contains:**
  - `LoginForm.cs`: Admin authentication
  - `RegistrationForm.cs`: Complete student CRUD with 4 GroupBoxes and full validation
  - `StudentRecordsForm.cs`: Live search directory, filtering, and CSV export
  - `ManageUsersForm.cs`: Multi-user administrative account management
  - `Program.cs`: Application Composition Root
  - `AppConfig.cs`: Configuration settings
- **Rule:** Calls repository interface methods. Never writes SQL directly.

---

## Form Capabilities Overview

| Form | Purpose | Key Features |
|---|---|---|
| `LoginForm` | User authentication | Validates against `Logins` table, error dialogs, Enter key trigger |
| `RegistrationForm` | Student registration management | Register, Update, Clear, Delete, Search by RegNo, 4 GroupBoxes, input validation |
| `StudentRecordsForm` | Student records directory | Live search, gender filtering, record count, double-click to load, CSV export |
| `ManageUsersForm` | Administrative account manager | Add new accounts, update passwords, delete users (Admin protected) |

---

## Docker Commands Reference

```bash
# Start container in background
docker compose -f docker/docker-compose.yml up -d

# Stop container (preserves database data volume)
docker compose -f docker/docker-compose.yml stop

# Stop container and wipe data (full database reset)
docker compose -f docker/docker-compose.yml down -v

# View container logs
docker compose -f docker/docker-compose.yml logs -f sqlserver
```

---

## Changing the Connection String

Edit [`src/CampusOne.App/AppConfig.cs`](../src/CampusOne.App/AppConfig.cs):

```csharp
public const string ConnectionString =
    "Server=localhost,1433;Database=Student;User Id=sa;Password=CampusOne@2024;TrustServerCertificate=True;";
```

---

## Build Verification

To verify the entire solution builds cleanly across all layers:

```bash
dotnet build --configuration Release
```
Expected output:
```
Build succeeded.
    0 Warning(s)
    0 Error(s)
```
