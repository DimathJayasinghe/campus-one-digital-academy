using CampusOne.Core.Models;
using CampusOne.Data;
using CampusOne.Data.Repositories;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("=============================================================");
Console.WriteLine(" Campus One Digital Academy – Live End-to-End System Checker");
Console.WriteLine("=============================================================");
Console.ResetColor();

const string connStr = "Server=localhost,1433;Database=Student;User Id=sa;Password=CampusOne@2024;TrustServerCertificate=True;";
var factory   = new DbConnectionFactory(connStr);
var loginRepo = new LoginRepository(factory);
var studRepo  = new StudentRepository(factory);

var allPassed = true;

void Test(string name, Action action)
{
    try
    {
        action();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($" [PASS] {name}");
        Console.ResetColor();
    }
    catch (Exception ex)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($" [FAIL] {name}: {ex.Message}");
        Console.ResetColor();
        allPassed = false;
    }
}

// ── 1. Database Connectivity ────────────────────────────────────────────────
Test("Database Connection Factory opens connection", () =>
{
    using var conn = factory.Create();
    if (conn.State != System.Data.ConnectionState.Open)
        throw new Exception("Connection is not open");
});

// ── 2. Authentication ───────────────────────────────────────────────────────
Test("Authenticate with valid Admin credentials", () =>
{
    var user = loginRepo.Authenticate("Admin", "Campusone@123");
    if (user == null || user.Username != "Admin")
        throw new Exception("Failed to authenticate Admin user");
});

Test("Authenticate rejects invalid credentials", () =>
{
    var user = loginRepo.Authenticate("Admin", "WrongPassword");
    if (user != null)
        throw new Exception("Security breach: invalid password was accepted");
});

Test("Authenticate rejects non-existent username", () =>
{
    var user = loginRepo.Authenticate("UnknownUser", "Campusone@123");
    if (user != null)
        throw new Exception("Security breach: unknown user was accepted");
});

// ── 3. Multi-User Management ────────────────────────────────────────────────
Test("Create and verify new user account", () =>
{
    var created = loginRepo.CreateUser("TestInstructor", "Pass@123");
    if (!created) throw new Exception("Failed to create new user");

    var authed = loginRepo.Authenticate("TestInstructor", "Pass@123");
    if (authed == null) throw new Exception("New user cannot authenticate");
});

Test("Update password for user", () =>
{
    var updated = loginRepo.UpdatePassword("TestInstructor", "NewPass@456");
    if (!updated) throw new Exception("Failed to update password");

    var authed = loginRepo.Authenticate("TestInstructor", "NewPass@456");
    if (authed == null) throw new Exception("User cannot authenticate with new password");
});

Test("Delete user account (and Admin protection)", () =>
{
    var adminDeleted = loginRepo.DeleteUser("Admin");
    if (adminDeleted) throw new Exception("Admin account should be protected from deletion!");

    var testDeleted = loginRepo.DeleteUser("TestInstructor");
    if (!testDeleted) throw new Exception("Failed to delete test user");
});

// ── 4. Student CRUD Operations ──────────────────────────────────────────────
const int testRegNo = 99999;

Test("Student CRUD: Add (Register) student", () =>
{
    // Clean up if previous run left it
    studRepo.Delete(testRegNo);

    var s = new Student
    {
        RegNo       = testRegNo,
        FirstName   = "Kamal",
        LastName    = "Perera",
        DateOfBirth = new DateTime(2005, 5, 20),
        Gender      = "Male",
        Address     = "123 Galle Road, Colombo 03",
        Email       = "kamal.perera@example.com",
        MobilePhone = 771234567,
        HomePhone   = 112345678,
        ParentName  = "Sunil Perera",
        Nic         = "200512345678",
        ContactNo   = 777654321
    };

    studRepo.Add(s);
});

Test("Student CRUD: Read (Search) by Reg No", () =>
{
    var s = studRepo.GetByRegNo(testRegNo);
    if (s == null) throw new Exception("Student not found by Reg No");
    if (s.FirstName != "Kamal" || s.LastName != "Perera") throw new Exception("Student name mismatch");
    if (s.Gender != "Male") throw new Exception("Gender mismatch");
    if (s.MobilePhone != 771234567) throw new Exception("Mobile phone mismatch");
    if (s.FullName != "Kamal Perera") throw new Exception("FullName property mismatch");
});

Test("Student CRUD: Get all Reg Nos list", () =>
{
    var regNos = studRepo.GetAllRegNos().ToList();
    if (!regNos.Contains(testRegNo))
        throw new Exception("RegNos list does not contain registered student");
});

Test("Student CRUD: Update student record", () =>
{
    var s = studRepo.GetByRegNo(testRegNo)!;
    s.Address     = "456 Kandy Road, Colombo";
    s.MobilePhone = 779998888;
    studRepo.Update(s);

    var updated = studRepo.GetByRegNo(testRegNo)!;
    if (updated.Address != "456 Kandy Road, Colombo" || updated.MobilePhone != 779998888)
        throw new Exception("Student update did not persist correctly");
});

Test("Student CRUD: GetAll returns list including student", () =>
{
    var all = studRepo.GetAll().ToList();
    if (!all.Any(st => st.RegNo == testRegNo))
        throw new Exception("GetAll did not return test student");
});

Test("Student CRUD: Delete student record", () =>
{
    studRepo.Delete(testRegNo);
    var deleted = studRepo.GetByRegNo(testRegNo);
    if (deleted != null)
        throw new Exception("Student still exists after deletion");
});

Console.WriteLine("-------------------------------------------------------------");
if (allPassed)
{
    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine(" ALL VERIFICATION TESTS PASSED SUCCESSFULLY! (0 Failures)");
    Console.ResetColor();
}
else
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(" SOME TESTS FAILED.");
    Console.ResetColor();
}
Console.WriteLine("=============================================================");
