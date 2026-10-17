using CampusOne.App;
using CampusOne.App.Forms;
using CampusOne.Data;
using CampusOne.Data.Repositories;

/// <summary>
/// Application entry point.
/// All dependencies are wired here (manual DI – no IoC container needed for this project size).
/// </summary>
internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();

        // Wire up dependencies (Composition Root pattern)
        var factory   = new DbConnectionFactory(AppConfig.ConnectionString);
        var loginRepo = new LoginRepository(factory);

        Application.Run(new LoginForm(loginRepo));
    }
}