using ValveModHub.Common.Model.A2S;

namespace ValveModHub.Common.Model;

public class Game
{
    public string? Name { get; set; }
    public string? GameDir { get; set; }
    public string? Filters { get; set; }
    public string? Icon { get; set; }
    public ulong? AppId { get; set; }
    public bool? UsesA2S { get; set; }
    public MasterServer? MasterServer { get; set; }
}