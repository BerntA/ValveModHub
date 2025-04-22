using Microsoft.Extensions.Caching.Memory;
using System.Collections.Concurrent;
using System.Net;
using System.Text.Json.Nodes;
using ValveModHub.Common.External;
using ValveModHub.Common.Model;
using ValveModHub.Common.Model.A2S;
using ValveModHub.Common.Utils;

namespace ValveModHub.Server.Services;

public class SteamServerBrowserApiService
{
    private readonly HttpClient _httpClient;
    private readonly IMemoryCache _cache;
    private readonly string _apiKey;
    private readonly int _querySize;

    public SteamServerBrowserApiService(IConfiguration config, IMemoryCache memoryCache)
    {
        var httpClientHandler = new HttpClientHandler();
        httpClientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;

        _httpClient = new HttpClient(httpClientHandler);
        _httpClient.BaseAddress = new Uri(config["SteamAPIUrl"]);

        _cache = memoryCache;
        _apiKey = config["SteamAPIKey"];
        _querySize = int.TryParse(config["QuerySize"], out var size) ? size : 1000;
    }

    public async Task<List<GameServerItem>> FetchServers(Game? game, int timeoutServers = 1500, int timeoutMasterServer = 15000)
    {
        if (game is null)
            return [];

        var key = $"Servers-{game.AppId}-{game.Name}".ToUpper();

        return await _cache.GetOrCreateAsync(key, async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1);

            if ((game.UsesA2S ?? false) && game.MasterServer.HasValue) // uses legacy protocols (A2S), slower, but needed for some games.
            {
                var legacyServers = await A2SQuery.QueryServerList(game.MasterServer.Value, game, timeoutMasterServer);
                return await QueryServers(game, legacyServers.Select(s => s.Address).ToList(), timeoutServers);
            }
            else
            {
                var gamedirFilter = string.IsNullOrEmpty(game.GameDir) ? string.Empty : $"\\gamedir\\{game.GameDir}";
                var extraFilters = string.IsNullOrEmpty(game.Filters) ? string.Empty : game.Filters;

                return await Fetch<GameServerItem>(
                    $"IGameServersService/GetServerList/v1/?key={_apiKey}&limit={_querySize}&filter=appid\\{game.AppId}{gamedirFilter}{extraFilters}"
                );
            }
        });
    }

    private async Task<List<T>> Fetch<T>(string url)
    {
        try
        {
            using var request = await _httpClient.GetAsync(url);

            request.EnsureSuccessStatusCode();

            using var stream = await request.Content.ReadAsStreamAsync();
            var jsonNode = await JsonNode.ParseAsync(stream);

            if (jsonNode is null || jsonNode["response"] is null || jsonNode["response"]["servers"] is null)
                return [];

            return JsonUtils.DeserializeObject<List<T>>(jsonNode["response"]["servers"].ToJsonString());
        }
        catch
        {
            return [];
        }
    }

    private static bool IsServerValid(Game game, ServerInfo? server)
    {
        if (server is null || !server.HasValue)
            return false;

        if (server.Value.MaxPlayers > 128 || server.Value.MaxPlayers <= 1 || server.Value.Players > server.Value.MaxPlayers)
            return false;

        if (game.MasterServer.HasValue && game.MasterServer.Value == MasterServer.GoldSrc && !server.Value.Version.EndsWith("/Stdio"))
            return false;

        return true;
    }

    private static async Task<List<GameServerItem>> QueryServers(Game game, List<string> servers, int timeout = 1500)
    {
        var items = new ConcurrentBag<GameServerItem>();
        await Parallel.ForEachAsync(servers, async (address, _) =>
        {
            var obj = await A2SQuery.QueryServerInfo(address, timeout);
            if (IsServerValid(game, obj))
                items.Add(obj.Value.MapToGameServerItem(game));
        });
        return [.. items];
    }
}