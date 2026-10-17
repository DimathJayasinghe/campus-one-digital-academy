# Campus One Digital Academy

> Student Management System — L3 Diploma in Information Technology Final Project  
> ESOFT Metro Campus · Campus One Digital Academy

[![.NET](https://img.shields.io/badge/.NET-10.0-blue)](https://dotnet.microsoft.com)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-2022-red)](https://hub.docker.com/_/microsoft-mssql-server)
[![Docker](https://img.shields.io/badge/Docker-ready-blue)](https://www.docker.com)
[![Platform](https://img.shields.io/badge/Platform-Windows%20%7C%20Linux%20Build-lightgrey)](https://learn.microsoft.com/en-us/dotnet/desktop/winforms)

---

## Overview

A robust, enterprise-grade Windows Forms desktop application built in C# for Campus One Digital Academy. It features multi-layered architecture, secure parameterized ADO.NET data access, Dockerized SQL Server 2022, input validation, and extended management utilities.

### Application Forms

| Form | Purpose | Features |
|---|---|---|
| **1. Login Form** | System Authentication | Validates against SQL database `Logins` table, Clear fields, Exit confirmation, Enter-key support |
| **2. Registration Form** | Student Records CRUD | Complete CRUD (Register, Update, Clear, Delete), 4 GroupBoxes, search by RegNo, input validation |
| **3. Student Directory** | Records & Reporting *(Bonus)* | Live search, gender filter, student counter, double-click to load, **Export to CSV** |
| **4. User Management** | Multi-User Admin *(Bonus)* | Add system users, change passwords, delete users, protection for primary Admin account |

---

## Quick Start

### 1. Prerequisites
- [.NET SDK 10.0+](https://dotnet.microsoft.com/download)
- [Docker Desktop](https://www.docker.com/products/docker-desktop)

### 2. Start SQL Server 2022 in Docker

Execute the startup script for your operating system to launch the container and automatically initialize the `Student` database and tables:

- **Linux / macOS:**
  ```bash
  bash scripts/db-start.sh
  ```
- **Windows (PowerShell):**
  ```powershell
  .\scripts\db-start.ps1
  ```
- **Windows (Command Prompt):**
  ```cmd
  scripts\db-start.bat
  ```

### 3. Build & Run Application

```bash
# Verify build
dotnet build --configuration Release

# Run on Windows
dotnet run --project src/CampusOne.App
```

**Default Admin Credentials:**
- **Username:** `Admin`
- **Password:** `Campusone@123`

---

## Project Architecture

Built with a 3-tier Layered Architecture adhering strictly to SOLID principles:

```
┌────────────────────────────────────────┐
│            CampusOne.App               │  WinForms UI (Forms, Controls, Validation)
└───────────────────┬────────────────────┘
                    │ references
       ┌────────────┴────────────┐
       ▼                         ▼
┌──────────────┐          ┌──────────────┐
│CampusOne.Core│ ◄─────── │CampusOne.Data│  ADO.NET Repositories & Connection Factory
└──────────────┘implements└──────┬───────┘
  Domain Models &                │
  Interfaces                     ▼
                          ┌──────────────┐
                          │  SQL Server  │  Docker Container (Port 1433)
                          └──────────────┘
```

---

## Design Patterns Used

| Pattern | Implementation | Purpose |
|---|---|---|
| **Repository** | `IStudentRepository`, `ILoginRepository` | Isolates all raw SQL queries from UI layers |
| **Factory Method** | `DbConnectionFactory` | Centralizes connection creation and security configuration |
| **Composition Root** | `Program.cs` | Pure dependency injection wiring at entry point |
| **Dependency Inversion** | Core Interfaces | Decouples high-level UI from low-level data layer |
| **Observer (Events)** | `LogoutRequested`, `StudentSelected` | Clean, decoupled inter-form communication |

---

## Documentation

- [`docs/ARCHITECTURE.md`](docs/ARCHITECTURE.md) — Comprehensive architectural specifications & diagrams
- [`docs/DATABASE_SCHEMA.md`](docs/DATABASE_SCHEMA.md) — Database schema, ERD, and SQL tables
- [`docs/DESIGN_PATTERNS.md`](docs/DESIGN_PATTERNS.md) — Design patterns with C# code examples
- [`docs/DEVELOPER_GUIDE.md`](docs/DEVELOPER_GUIDE.md) — Developer handbook and troubleshooting guide
- [`docs/AGENT_HANDOVER.md`](docs/AGENT_HANDOVER.md) — Context handover log for AI agents

---

## Author & Academic Information

- **Student Name:** Dimath Jayasinghe
- **Programme:** L3 - Diploma in Information Technology
- **Institution:** ESOFT Metro Campus
- **Project Title:** Campus One Digital Academy
