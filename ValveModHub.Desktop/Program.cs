using ValveModHub.Desktop.Forms;
using ValveModHub.Desktop.Services;

namespace ValveModHub.Desktop;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        SteamLoaderService.Load();
        Application.Run(new MainForm());
        SteamLoaderService.Shutdown();
    }
}