using CampusOne.Core.Models;

namespace CampusOne.Core.Interfaces;

/// <summary>
/// Contract for authentication and user account management data-access.
/// </summary>
public interface ILoginRepository
{
    /// <summary>Returns the user if username and password match, otherwise null.</summary>
    LoginUser? Authenticate(string username, string password);

    /// <summary>Retrieves all registered user accounts.</summary>
    IEnumerable<LoginUser> GetAllUsers();

    /// <summary>Creates a new user account. Returns true if successful, false if username exists.</summary>
    bool CreateUser(string username, string password);

    /// <summary>Updates password for an existing user account.</summary>
    bool UpdatePassword(string username, string newPassword);

    /// <summary>Deletes a user account. Cannot delete the default 'Admin' account.</summary>
    bool DeleteUser(string username);
}
