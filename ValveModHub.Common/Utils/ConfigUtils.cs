using ValveModHub.Common.Model;

namespace ValveModHub.Common.Utils;

public static class ConfigUtils
{
    public static AppSettings Settings { get; set; }

    static ConfigUtils()
    {
        var data = FileUtils.LoadDataFromFile<AppSettings>($"{AppContext.BaseDirectory}\\assets\\data\\manifest.json");
        if (data is null)
            throw new ApplicationException("Unable to load app manifest file!");
        Settings = data;
    }
}