using Microsoft.AspNetCore.Mvc;
using System.Net;
using ValveModHub.Common.Utils;
using ValveModHub.Server.Services;

namespace ValveModHub.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GameServerController : ControllerBase
{
    private readonly SteamServerBrowserApiService _serverBrowserService;
    private readonly SteamPlayerDetailApiService _playerInfoService;

    public GameServerController(
        SteamServerBrowserApiService serverBrowserService,
        SteamPlayerDetailApiService playerInfoService
    )
    {
        _serverBrowserService = serverBrowserService;
        _playerInfoService = playerInfoService;
    }

    [HttpGet("{name}")]
    public async Task<IActionResult> GetServers(string name)
    {
        var game = GameList.GetGameByName(name);
        if (game is null)
            return new NotFoundObjectResult($"Unable to retrieve any servers for: {name}");

        var servers = await _serverBrowserService.FetchServers(game);
        return new OkObjectResult(servers);
    }

    [HttpGet("players/{address}")]
    public async Task<IActionResult> GetPlayers(string address)
    {
        if (string.IsNullOrEmpty(address) || !IPEndPoint.TryParse(address, out var host))
            return new BadRequestObjectResult($"{address} is not a valid ip:port!");

        var players = await _playerInfoService.FetchPlayerDetails(host.ToString());
        return new OkObjectResult(players);
    }
}