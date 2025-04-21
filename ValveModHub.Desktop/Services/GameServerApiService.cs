using Microsoft.Extensions.Caching.Memory;
using System.Diagnostics;
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

        return await GlobalCache.Cache.GetOrCreateAsync<List<GameServerItem>>($"game-servers-{game.Name.ToLower()}",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(2);
                var servers = await Fetch<GameServerItem>($"{ConfigUtils.Settings.Server.Url}/api/gameserver/{game.Name}");
                return [.. servers.OrderByDescending(o => o.CurrentPlayers)];
            });
    }

    public static async Task<List<PlayerItem>> GetPlayers(GameServerItem? server)
    {
        if (server is null)
            return [];

        return await GlobalCache.Cache.GetOrCreateAsync($"game-server-{server.Address}-players",
            async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);
                var players = await Fetch<PlayerItem>($"{ConfigUtils.Settings.Server.Url}/api/gameserver/players/{server.Address}");
                return players;
            });
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

    public static void ConnectToServer(GameServerItem? server)
    {
        if (server is null)
            return;

        Process.Start("explorer.exe", $"steam://connect/{server.Address}");
    }
}