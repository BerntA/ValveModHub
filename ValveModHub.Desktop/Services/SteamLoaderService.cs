using Steamworks;

namespace ValveModHub.Desktop.Services;

public static class SteamLoaderService
{
    public static bool IsLoaded { get; set; }

    static SteamLoaderService()
    {
        IsLoaded = false;
    }

    public static void Load()
    {
        if (IsLoaded)
            return;

        try
        {
            SteamClient.Init(211);
            IsLoaded = true;
        }
        catch
        {
            // ignored, steam is not running..
        }
    }

    public static void Shutdown()
    {
        if (!IsLoaded)
            return;

        try
        {
            SteamClient.Shutdown();
        }
        catch
        {
            // failed shutdown, Volvo pls?
        }

        IsLoaded = false;
    }
}