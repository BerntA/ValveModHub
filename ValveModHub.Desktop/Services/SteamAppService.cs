using Microsoft.Win32;
using ValveModHub.Common.Model;

namespace ValveModHub.Desktop.Services;

public static class SteamAppService
{
    public static string SourceModPath { get; set; }
    public static string GoldSrcPath { get; set; }

    static SteamAppService()
    {
        SourceModPath = GetRegValue("Software\\Valve\\Steam", "SourceModInstallPath", out string sourceModPath) ? sourceModPath : string.Empty;
        GoldSrcPath = GetRegValue("Software\\Valve\\Steam", "ModInstallPath", out string goldSrcPath) ? goldSrcPath : string.Empty;
    }

    public static bool IsGameInstalled(ulong appId)
    {
        return GetRegValue($"Software\\Valve\\Steam\\Apps\\{appId}", "Installed", out int state) && state > 0;
    }

    public static bool IsModInstalled(Game game)
    {
        if (string.IsNullOrEmpty(game.GameDir))
            return IsGameInstalled(game.AppId ?? 0);

        var path = game.MasterServer == Common.Model.A2S.MasterServer.GoldSrc ? GoldSrcPath : SourceModPath;
        return Directory.Exists($"{path}/{game.GameDir}");
    }

    private static bool GetRegValue<T>(string key, string value, out T result)
    {
        try
        {
            using var registryKey = Registry.CurrentUser.OpenSubKey(key);

            if (registryKey is null)
            {
                result = default;
                return false;
            }

            var regValue = registryKey.GetValue(value);

            if (regValue is null)
            {
                result = default;
                return false;
            }

            result = (T)Convert.ChangeType(regValue, typeof(T));
            return true;
        }
        catch
        {
            result = default;
            return false;
        }
    }
}