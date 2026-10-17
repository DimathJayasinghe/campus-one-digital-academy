# Campus One Digital Academy – Design Patterns Reference

> [!NOTE]
> This document explains every design pattern used in this project, **why** it was chosen, and where to find it in the codebase.

---

## Patterns At a Glance

| # | Pattern | Category | File(s) |
|---|---|---|---|
| 1 | Repository | Structural | `IStudentRepository`, `StudentRepository`, `ILoginRepository`, `LoginRepository` |
| 2 | Factory Method | Creational | `DbConnectionFactory` |
| 3 | Composition Root | Architectural | `Program.cs` |
| 4 | Dependency Inversion (DIP) | SOLID | `IStudentRepository`, `ILoginRepository` in `CampusOne.Core` |
| 5 | Observer / Event | Behavioural | `RegistrationForm.LogoutRequested`, `StudentRecordsForm.StudentSelected` |

---

## 1. Repository Pattern

### What it is
Wraps all database access inside a dedicated class that abstracts queries away from UI presentation logic. The caller consumes standard C# methods without writing or knowing the underlying SQL syntax.

### Why we used it
WinForms UI controls should never directly execute raw SQL queries. If database tables or column types change, changes are isolated strictly to the repository implementation rather than scattered across dozens of button click handlers.

### Where it is
- Contract: `src/CampusOne.Core/Interfaces/IStudentRepository.cs` and `ILoginRepository.cs`
- Implementation: `src/CampusOne.Data/Repositories/StudentRepository.cs` and `LoginRepository.cs`

### Code sketch
```csharp
// Core/Interfaces/IStudentRepository.cs
public interface IStudentRepository
{
    IEnumerable<Student> GetAll();
    Student? GetByRegNo(int regNo);
    void Add(Student student);
    void Update(Student student);
    void Delete(int regNo);
    IEnumerable<int> GetAllRegNos();
}

// Data/Repositories/StudentRepository.cs
public class StudentRepository : IStudentRepository
{
    public Student? GetByRegNo(int regNo)
    {
        using var conn = _factory.Create();
        using var cmd  = new SqlCommand("SELECT * FROM Registration WHERE regNo = @regNo", conn);
        cmd.Parameters.AddWithValue("@regNo", regNo);
        using var reader = cmd.ExecuteReader();
        return reader.Read() ? MapRow(reader) : null;
    }
}
```

---

## 2. Factory Method Pattern

### What it is
A specialized factory class responsible for instantiating and opening database connections with the configured connection string.

### Why we used it
Centralizes connection string management, prevents code duplication across multiple repositories, and ensures connection state is consistently initialized and safely wrapped in `using` statements for automatic resource disposal.

### Where it is
`src/CampusOne.Data/DbConnectionFactory.cs`

### Code sketch
```csharp
public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString) =>
        _connectionString = connectionString;

    public SqlConnection Create()
    {
        var conn = new SqlConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
```

---

## 3. Composition Root Pattern

### What it is
A single, unified application location where all concrete dependencies are instantiated and wired together before the primary UI forms run.

### Why we used it
Eliminates scattered `new` expressions and hidden dependencies inside UI classes. Forms receive their dependencies via constructors, simplifying code understanding and maintenance.

### Where it is
`src/CampusOne.App/Program.cs`

### Code sketch
```csharp
[STAThread]
private static void Main()
{
    ApplicationConfiguration.Initialize();

    // Wire up dependencies in one location
    var factory   = new DbConnectionFactory(AppConfig.ConnectionString);
    var loginRepo = new LoginRepository(factory);

    Application.Run(new LoginForm(loginRepo));
}
```

---

## 4. Dependency Inversion Principle (DIP)

### What it is
> *"High-level modules should not depend on low-level modules. Both should depend on abstractions."*

Forms in `CampusOne.App` depend exclusively on interfaces defined in `CampusOne.Core`, not on concrete SQL implementations in `CampusOne.Data`.

### Why we used it
Allows changing the database engine or substituting mock repositories without altering a single line of UI presentation code.

---

## 5. Observer Pattern (C# Events)

### What it is
Allows one object to broadcast state changes or user actions to listeners without tight coupling or circular dependencies.

### Why we used it
- `RegistrationForm.LogoutRequested`: Signals `LoginForm` to re-appear when the user clicks Logout, preventing circular references between forms.
- `StudentRecordsForm.StudentSelected`: Notifies `RegistrationForm` when a row is selected/double-clicked, passing the selected `regNo` to automatically populate the registration form.

### Code sketch
```csharp
// StudentRecordsForm broadcasts:
public event Action<int>? StudentSelected;

private void BtnOpen_Click(object? sender, EventArgs e)
{
    if (int.TryParse(dgvStudents.SelectedRows[0].Cells["RegNo"].Value?.ToString(), out var regNo))
    {
        StudentSelected?.Invoke(regNo);
        Close();
    }
}

// RegistrationForm listens:
var recordsForm = new StudentRecordsForm(_studentRepo);
recordsForm.StudentSelected += regNo => LoadStudentByRegNo(regNo);
recordsForm.ShowDialog(this);
```
