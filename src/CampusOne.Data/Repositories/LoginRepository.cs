using CampusOne.Core.Interfaces;
using CampusOne.Core.Models;
using Microsoft.Data.SqlClient;

namespace CampusOne.Data.Repositories;

/// <summary>ADO.NET implementation of <see cref="ILoginRepository"/>.</summary>
public class LoginRepository : ILoginRepository
{
    private readonly DbConnectionFactory _factory;

    public LoginRepository(DbConnectionFactory factory) => _factory = factory;

    /// <inheritdoc/>
    public LoginUser? Authenticate(string username, string password)
    {
        const string sql =
            "SELECT id, username, password FROM Logins " +
            "WHERE username = @username AND password = @password";

        using var conn = _factory.Create();
        using var cmd  = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@password", password);

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return null;

        return new LoginUser
        {
            Id       = reader.GetInt32(0),
            Username = reader.GetString(1),
            Password = reader.GetString(2)
        };
    }
}
