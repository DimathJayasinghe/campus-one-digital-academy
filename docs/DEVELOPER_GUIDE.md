# Campus One Digital Academy – Developer Guide

---

## Prerequisites

| Tool | Minimum version | Purpose |
|---|---|---|
| .NET SDK | 10.0 | Build & run the app |
| Docker Desktop | 24+ | Run SQL Server |
| Visual Studio 2022 | 17.8+ | WinForms designer (Windows) |
| VS Code + C# Dev Kit | Latest | Edit on Linux/Mac |
| `gh` CLI (optional) | 2.x | GitHub operations |

---

## Quick Start

```bash
# 1. Clone the repo
git clone https://github.com/DimathJayasinghe/campus-one-digital-academy.git
cd campus-one-digital-academy

# 2. Start SQL Server in Docker and seed the database
bash scripts/db-start.sh

# 3. (Windows) Build and run
dotnet run --project src/CampusOne.App

# 4. Restore NuGet packages (if needed)
dotnet restore
```

**Login credentials:** `Admin` / `Campusone@123`

---

## Project Layers – What Goes Where

### `CampusOne.Core` — Domain Layer
- **Contains:** Models, Interfaces
- **Rule:** Zero external dependencies. No NuGet packages, no SQL, no UI.
- **When to add here:** New domain model (e.g. `Course`), or a new repository contract.

### `CampusOne.Data` — Data Layer
- **Contains:** `DbConnectionFactory`, repository implementations
- **Rule:** Only talks to SQL. No UI logic, no message boxes.
- **When to add here:** New repository (e.g. `CourseRepository`), or new SQL query.

### `CampusOne.App` — UI Layer
- **Contains:** WinForms (`LoginForm`, `RegistrationForm`), `Program.cs`, `AppConfig.cs`
- **Rule:** Calls repository methods. Never writes SQL directly.
- **When to add here:** New form, new button handler.

---

## How to Add a New Feature

### Example: Add a "Courses" module

1. **Add model** in Core:
   ```csharp
   // src/CampusOne.Core/Models/Course.cs
   public class Course { public int Id; public string Name; }
   ```

2. **Add interface** in Core:
   ```csharp
   // src/CampusOne.Core/Interfaces/ICourseRepository.cs
   public interface ICourseRepository { IEnumerable<Course> GetAll(); }
   ```

3. **Add SQL** in `init.sql`:
   ```sql
   CREATE TABLE Courses (id INT PRIMARY KEY IDENTITY, name VARCHAR(100));
   ```

4. **Add repository** in Data:
   ```csharp
   // src/CampusOne.Data/Repositories/CourseRepository.cs
   public class CourseRepository : ICourseRepository { ... }
   ```

5. **Wire in `Program.cs`:**
   ```csharp
   var courseRepo = new CourseRepository(factory);
   var courseForm = new CourseForm(courseRepo);
   ```

6. **Create the form** in App:
   ```csharp
   // src/CampusOne.App/Forms/CourseForm.cs
   public class CourseForm : Form { ... }
   ```

---

## Repository Pattern – Key Points

```
IStudentRepository (Core)       ← Interface (contract)
       ↑
StudentRepository (Data)        ← SQL implementation
       ↑
RegistrationForm (App)          ← Uses the interface, not the class
```

The form **only knows about the interface**. This means:
- You can unit test the form by passing a fake repository
- You can swap to a different database without changing the form

---

## Form Coding Conventions

| Convention | Example |
|---|---|
| Controls declared as fields | `private TextBox txtFirstName = null!;` |
| Layout in `InitializeComponent()` | No `.designer.cs` file needed |
| Events wired at end of `InitializeComponent` | `btnLogin.Click += BtnLogin_Click;` |
| All DB calls in `try/catch` | Show `MessageBox` on exception |
| Helper factory methods for repeated controls | `MakeLabel(...)`, `MakeButton(...)` |

---

## Changing the Connection String

Edit [`src/CampusOne.App/AppConfig.cs`](file:///home/dimathrj/Documents/dev_projects/padji/src/CampusOne.App/AppConfig.cs):

```csharp
public const string ConnectionString =
    "Server=localhost,1433;Database=Student;User Id=sa;Password=CampusOne@2024;TrustServerCertificate=True;";
```

For a different SQL Server instance, change `localhost,1433` to your server address.

---

## Docker Commands Reference

```bash
# Start the database container
docker compose -f docker/docker-compose.yml up -d

# Stop the container (keeps data)
docker compose -f docker/docker-compose.yml stop

# Stop and remove data (full reset)
docker compose -f docker/docker-compose.yml down -v

# View container logs
docker compose -f docker/docker-compose.yml logs -f sqlserver

# Connect via sqlcmd inside container
docker exec -it campusone_db /opt/mssql-tools18/bin/sqlcmd \
  -S localhost -U sa -P "CampusOne@2024" -C -N
```

---

## Troubleshooting

| Problem | Fix |
|---|---|
| `Cannot open server 'localhost'` | Run `bash scripts/db-start.sh` first |
| `Login failed for user 'sa'` | Check password in `AppConfig.cs` matches `docker-compose.yml` |
| `regNo already exists` | Reg No is the Primary Key — use a unique number |
| Build fails on Linux | Ensure `<EnableWindowsTargeting>true</EnableWindowsTargeting>` is in App `.csproj` |
| WinForms won't run on Linux | WinForms requires Windows to *run* (build-only on Linux) |

---

## Git Workflow

```bash
# Create a feature branch
git checkout -b feature/add-courses

# Stage and commit
git add .
git commit -m "feat: add course management form"

# Push to GitHub
git push origin feature/add-courses
```
