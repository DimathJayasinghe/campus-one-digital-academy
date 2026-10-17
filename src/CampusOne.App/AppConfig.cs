namespace CampusOne.App;

/// <summary>
/// Central configuration. Change the connection string here if needed.
/// For Docker: Server=localhost,1433;Database=Student;User Id=sa;Password=CampusOne@2024;TrustServerCertificate=True;
/// </summary>
internal static class AppConfig
{
    public const string ConnectionString =
        "Server=localhost,1433;Database=Student;User Id=sa;Password=CampusOne@2024;TrustServerCertificate=True;";
}
