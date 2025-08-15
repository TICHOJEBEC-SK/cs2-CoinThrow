using CounterStrikeSharp.API.Core;
using System.Text.Json.Serialization;

namespace CoinThrow
{
    public class Config : BasePluginConfig
    {
        [JsonPropertyName("DBDatabase")] public string DbDatabase { get; set; } = "database";
        [JsonPropertyName("DBUser")] public string DbUser { get; set; } = "user";
        [JsonPropertyName("DBPassword")] public string DbPassword { get; set; } = "password";
        [JsonPropertyName("DBHost")] public string DbHost { get; set; } = "localhost";
        [JsonPropertyName("DBPort")] public int DbPort { get; set; } = 3306;
    }
}