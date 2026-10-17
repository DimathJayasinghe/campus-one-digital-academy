using CampusOne.Core.Models;

namespace CampusOne.Core.Interfaces;

/// <summary>Contract for authentication data-access.</summary>
public interface ILoginRepository
{
    /// <summary>Returns the user if username and password match, otherwise null.</summary>
    LoginUser? Authenticate(string username, string password);
}
