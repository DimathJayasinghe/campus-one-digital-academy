using CampusOne.Core.Models;

namespace CampusOne.Core.Interfaces;

/// <summary>
/// Contract for student data-access operations.
/// Implementing this interface keeps the UI layer decoupled from the database.
/// </summary>
public interface IStudentRepository
{
    /// <summary>Returns every student in the database.</summary>
    IEnumerable<Student> GetAll();

    /// <summary>Finds a single student by registration number, or null if not found.</summary>
    Student? GetByRegNo(int regNo);

    /// <summary>Inserts a new student record.</summary>
    void Add(Student student);

    /// <summary>Updates an existing student record identified by RegNo.</summary>
    void Update(Student student);

    /// <summary>Deletes the student with the given registration number.</summary>
    void Delete(int regNo);

    /// <summary>Returns all registration numbers currently in the database.</summary>
    IEnumerable<int> GetAllRegNos();
}
