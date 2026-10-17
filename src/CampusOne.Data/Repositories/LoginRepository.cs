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

    /// <inheritdoc/>
    public IEnumerable<LoginUser> GetAllUsers()
    {
        var users = new List<LoginUser>();
        const string sql = "SELECT id, username, password FROM Logins ORDER BY username";

        using var conn = _factory.Create();
        using var cmd = new SqlCommand(sql, conn);
        using var reader = cmd.ExecuteReader();

        while (reader.Read())
        {
            users.Add(new LoginUser
            {
                Id = reader.GetInt32(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2)
            });
        }

        return users;
    }

    /// <inheritdoc/>
    public bool CreateUser(string username, string password)
    {
        const string checkSql = "SELECT COUNT(1) FROM Logins WHERE username = @username";
        const string insertSql = "INSERT INTO Logins (username, password) VALUES (@username, @password)";

        using var conn = _factory.Create();

        using (var checkCmd = new SqlCommand(checkSql, conn))
        {
            checkCmd.Parameters.AddWithValue("@username", username);
            var exists = (int)(checkCmd.ExecuteScalar() ?? 0) > 0;
            if (exists) return false;
        }

        using (var insertCmd = new SqlCommand(insertSql, conn))
        {
            insertCmd.Parameters.AddWithValue("@username", username);
            insertCmd.Parameters.AddWithValue("@password", password);
            insertCmd.ExecuteNonQuery();
        }

        return true;
    }

    /// <inheritdoc/>
    public bool UpdatePassword(string username, string newPassword)
    {
        const string sql = "UPDATE Logins SET password = @newPassword WHERE username = @username";

        using var conn = _factory.Create();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@username", username);
        cmd.Parameters.AddWithValue("@newPassword", newPassword);

        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }

    /// <inheritdoc/>
    public bool DeleteUser(string username)
    {
        // Protect default Admin from being deleted
        if (string.Equals(username, "Admin", StringComparison.OrdinalIgnoreCase))
            return false;

        const string sql = "DELETE FROM Logins WHERE username = @username";

        using var conn = _factory.Create();
        using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@username", username);

        var rows = cmd.ExecuteNonQuery();
        return rows > 0;
    }
}
