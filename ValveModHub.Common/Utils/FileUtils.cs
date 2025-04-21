namespace ValveModHub.Common.Utils;

public static class FileUtils
{
    public static T LoadDataFromFile<T>(string path)
    {
        if (!File.Exists(path))
            return default;

        var content = File.ReadAllText(path);
        if (string.IsNullOrEmpty(content))
            return default;

        return JsonUtils.DeserializeObject<T>(content);
    }
}