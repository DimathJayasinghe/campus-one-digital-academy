namespace CampusOne.Core.Models;

/// <summary>Represents a registered student.</summary>
public class Student
{
    public int RegNo { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public string Gender { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public long MobilePhone { get; set; }
    public long HomePhone { get; set; }
    public string ParentName { get; set; } = string.Empty;
    public string Nic { get; set; } = string.Empty;
    public long ContactNo { get; set; }

    /// <summary>Returns the student's full name.</summary>
    public string FullName => $"{FirstName} {LastName}";
}
