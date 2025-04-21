using Steamworks;
using ValveModHub.Desktop.Forms;

namespace ValveModHub.Desktop;

internal static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        try
        {
            SteamClient.Init(211);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Error");
        }

        Application.Run(new MainForm());

        SteamClient.Shutdown();
    }
}