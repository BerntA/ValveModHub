using System.Text.Json.Nodes;
using ValveModHub.Common.Model;
using ValveModHub.Common.Utils;

namespace ValveModHub.Desktop.Services;

public static class GameServerApiService
{
    private static readonly HttpClient _httpClient;

    static GameServerApiService()
    {
        _httpClient = new HttpClient();
    }

    public static async Task<List<GameServerItem>> GetServers(Game? game)
    {
        if (game is null)
            return [];

        return await Fetch<GameServerItem>($"http://localhost:3000/api/gameserver/{game.Name}");
    }

    private static async Task<List<T>> Fetch<T>(string url)
    {
        try
        {
            var content = await _httpClient.GetStringAsync(url);
            var jsonNode = JsonNode.Parse(content);

            if (jsonNode is null)
                return [];

            var data = JsonUtils.DeserializeObject<List<T>>(jsonNode.ToJsonString());
            return data;
        }
        catch
        {
            return [];
        }
    }
}