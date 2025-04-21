using ValveModHub.Common.Model;

namespace ValveModHub.Common.Utils;

public static class GameList
{
    static GameList()
    {
        var gameData = FileUtils.LoadDataFromFile<List<Game>>($"{AppContext.BaseDirectory}\\assets\\data\\games.json");
        Games = (gameData is null) ? [] : [.. gameData.OrderBy(g => g.Name)];
    }

    public static List<Game> Games { get; set; }

    public static Game? GetGameByIndex(int index)
    {
        if (index < 0 || index >= Games.Count)
            return null;

        return Games[index];
    }

    public static Game? GetGameByName(string name)
    {
        if (string.IsNullOrEmpty(name))
            return null;

        return Games
            .Where(g => g.Name is not null && g.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
            .FirstOrDefault();
    }
}