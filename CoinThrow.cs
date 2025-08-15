using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;

namespace CoinThrow
{
    public class CoinThrow : BasePlugin, IPluginConfig<Config>
    {
        private readonly Dictionary<string, DateTime> _lastCoinThrowTimes = new();
        private static readonly Random Random = new();
        private const int CooldownSeconds = 10;

        private Database? _database;

        public override string ModuleAuthor => "TICHOJEBEC";
        public override string ModuleName => "CoinThrow";
        public override string ModuleVersion => "v1.1";

        public Config Config { get; set; } = new();
        public void OnConfigParsed(Config config) => Config = config;

        public override void Load(bool hotReload)
        {
            _database = new Database(Config.DbHost, Config.DbPort, Config.DbDatabase, Config.DbUser, Config.DbPassword);
            _database.Initialize();

            Console.WriteLine("[CoinThrow] Plugin loaded successfully.");
        }

        private bool IsOnCooldown(string steamId, out double remainingSeconds)
        {
            if (_lastCoinThrowTimes.TryGetValue(steamId, out var lastThrow))
            {
                var elapsed = DateTime.Now - lastThrow;
                if (elapsed.TotalSeconds < CooldownSeconds)
                {
                    remainingSeconds = CooldownSeconds - elapsed.TotalSeconds;
                    return true;
                }
            }

            remainingSeconds = 0;
            return false;
        }

        private void UpdateLastCoinThrowTime(string steamId) =>
            _lastCoinThrowTimes[steamId] = DateTime.Now;

        [ConsoleCommand("css_cointhrow", "Throw a coin with the result being heads or tails")]
        public void OnCoinThrowCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player == null || player.PlayerPawn.Value == null || !player.IsValid)
                return;

            string steamId = player.SteamID.ToString();

            if (IsOnCooldown(steamId, out var remaining))
            {
                player.PrintToChat($"You can throw the coin only once every {ChatColors.DarkRed}{CooldownSeconds}{ChatColors.Default} seconds! ({remaining:F0}s left)");
                return;
            }

            string resultString = Random.Next(2) == 0 ? "Heads" : "Tails";

            Server.PrintToChatAll(
                $"Player {ChatColors.Green}{player.PlayerName}{ChatColors.Default} threw the coin and the result is {ChatColors.Green}{resultString}{ChatColors.Default}."
            );

            int totalThrows = _database?.IncrementPlayerThrows(steamId, player.PlayerName) ?? 0;

            Server.PrintToChatAll(
                $"His total number of throws is {ChatColors.Green}{totalThrows}x{ChatColors.Default}."
            );

            UpdateLastCoinThrowTime(steamId);
        }
    }
}
