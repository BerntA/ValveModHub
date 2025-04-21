using System.Text.Json.Serialization;

namespace ValveModHub.Common.Model.A2S;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum MasterServer
{
    GoldSrc,
    Source,
    DarkMessiah,
}