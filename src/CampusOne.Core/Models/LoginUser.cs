namespace CampusOne.Core.Models;

/// <summary>Represents a user who can log in to the system.</summary>
public class LoginUser
{
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}
