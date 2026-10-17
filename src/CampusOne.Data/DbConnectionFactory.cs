using Microsoft.Data.SqlClient;

namespace CampusOne.Data;

/// <summary>
/// Factory that creates open SQL connections.
/// Single responsibility: manage the connection string.
/// </summary>
public class DbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentNullException(nameof(connectionString));
        _connectionString = connectionString;
    }

    /// <summary>Opens and returns a new SqlConnection.</summary>
    public SqlConnection Create()
    {
        var conn = new SqlConnection(_connectionString);
        conn.Open();
        return conn;
    }
}
