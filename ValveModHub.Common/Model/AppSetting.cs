namespace ValveModHub.Common.Model;

public class Settings
{
    public string Version { get; set; }
    public string Url { get; set; }
}

public class AppSettings
{
    public Settings Client { get; set; }
    public Settings Server { get; set; }
}