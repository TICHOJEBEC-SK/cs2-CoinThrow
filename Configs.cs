using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Utils;
using System.Text.Json.Serialization;

namespace CoinThrow;


public class ConfigCT : BasePluginConfig
{
    [JsonPropertyName("DBDatabase")] public string DBDatabase { get; set; } = "database";
    [JsonPropertyName("DBUser")] public string DBUser { get; set; } = "user";
    [JsonPropertyName("DBPassword")] public string DBPassword { get; set; } = "password";
    [JsonPropertyName("DBHost")] public string DBHost { get; set; } = "localhost";
    [JsonPropertyName("DBPort")] public int DBPort { get; set; } = 3306;
}