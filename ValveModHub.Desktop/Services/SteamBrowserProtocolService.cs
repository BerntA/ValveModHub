using System.Diagnostics;
using System.Text;
using ValveModHub.Common.Model;

namespace ValveModHub.Desktop.Services;

public static class SteamBrowserProtocolService
{
    public static void ConnectToServer(GameServerItem? server)
    {
        if (server is null)
            return;

        Process.Start("explorer.exe", $"steam://connect/{server.Address}");
    }

    public static void LaunchGame(Game? game)
    {
        if (game is null)
            return;

        var bldr = new StringBuilder();
        bldr.Append($"steam://run/{game.AppId}");

        if (!string.IsNullOrEmpty(game.GameDir))
            bldr.Append($"//-game {game.GameDir}/");

        Process.Start("explorer.exe", bldr.ToString());
    }
}