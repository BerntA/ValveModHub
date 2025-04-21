using System.Text.Json.Serialization;
using ValveModHub.Common.Model.A2S;

namespace ValveModHub.Common.Model;

public class GameServerItem
{
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    [JsonPropertyName("addr")]
    public string? Address { get; set; }

    [JsonPropertyName("appid")]
    public ulong? AppId { get; set; }

    [JsonPropertyName("gamedir")]
    public string? GameDir { get; set; }

    [JsonPropertyName("players")]
    public int? CurrentPlayers { get; set; }

    [JsonPropertyName("max_players")]
    public int? MaxPlayers { get; set; }

    [JsonPropertyName("map")]
    public string? Map { get; set; }

    #region ServerInfo
    [JsonPropertyName("os")]
    public string? OperatingSystem { get; set; }

    [JsonPropertyName("version")]
    public string? Version { get; set; }

    [JsonPropertyName("secure")]
    public bool? IsVACEnabled { get; set; }

    [JsonPropertyName("dedicated")]
    public bool? IsDedicatedServer { get; set; }

    [JsonPropertyName("bots")]
    public int? Bots { get; set; }
    #endregion

    #region PlayerInfo
    [JsonIgnore]
    public List<PlayerInfo>? ActivePlayers { get; set; }

    [JsonIgnore]
    public bool ShouldShowDetails { get; set; } = false;
    #endregion
}