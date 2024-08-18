using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Commands.Targeting;
using CounterStrikeSharp.API.Modules.Utils;
using Nexd.MySQL;

namespace CoinThrow
{
    public partial class CoinThrow : BasePlugin, IPluginConfig<ConfigCT>
    {
        private Dictionary<string, DateTime> lastCoinThrowTimes = new Dictionary<string, DateTime>();
        private MySqlDb? mySql;
        public override string ModuleAuthor => "TICHOJEBEC";
        public override string ModuleName => "CoinThrow";
        public override string ModuleVersion => "v1.0";
        public ConfigCT Config { get; set; } = new ConfigCT();

        public void OnConfigParsed(ConfigCT config)
        {
            Config = config;
        }
        private static void GetPlayers(out List<CCSPlayerController> players)
        {
            players = Utilities.GetPlayers().Where(s => s.IsValid && s.PlayerPawn.Value != null).ToList();
        }
        public override void Load(bool hotReload)
        {
            InitializeDatabase();
            Console.WriteLine("CoinThrow plugin was successfully loaded!");
        }
        private void InitializeDatabase()
        {
            try
            {
                mySql = new MySqlDb(Config.DBHost, Config.DBUser, Config.DBPassword, Config.DBDatabase);
                mySql.ExecuteNonQueryAsync(@"
                    CREATE TABLE IF NOT EXISTS `cointhrow` (
                        `id` INT AUTO_INCREMENT PRIMARY KEY,
                        `player_steamid` VARCHAR(32) NOT NULL UNIQUE,
                        `player_name` VARCHAR(32) NOT NULL,
                        `counts` INT(11) NOT NULL DEFAULT 0
                    );
                ");
                Console.WriteLine($"Connection to the database {Config.DBHost} successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Connection to the database was unsuccessful: {ex.Message}.");
            }
        }
        private int GetPlayerThrows(string playerSteamID)
        {
            if (mySql == null)
                throw new InvalidOperationException("Database not initialized.");
            var result = mySql.Table("cointhrow")
                .Where(MySqlQueryCondition.New("player_steamid", "=", playerSteamID))
                .Select();
            if (result.Rows > 0)
            {
                return result.Get<int>(0, "counts");
            }
            return 0;
        }
        private bool CanPlayerThrowCoin(string steamID)
        {
            if (!lastCoinThrowTimes.ContainsKey(steamID))
            {
                return true;
            }
            DateTime lastThrowTime = lastCoinThrowTimes[steamID];
            TimeSpan timeSinceLastThrow = DateTime.Now - lastThrowTime;
            return timeSinceLastThrow.TotalSeconds >= 10;
        }
        private void UpdateLastCoinThrowTime(string steamID)
        {
            lastCoinThrowTimes[steamID] = DateTime.Now;
        }
        private void AddOrUpdateCoinThrowInDatabase(string playerSteamID, string playerName)
        {
            if (mySql == null)
                throw new InvalidOperationException("Database not initialized.");

            var result = mySql.Table("cointhrow")
                .Where(MySqlQueryCondition.New("player_steamid", "=", playerSteamID))
                .Select();

            if (result.Rows > 0)
            {
                int currentCount = Convert.ToInt32(result[0]["counts"]);
                currentCount++;
                mySql.Table("cointhrow")
                    .Where(MySqlQueryCondition.New("player_steamid", "=", playerSteamID))
                    .Update(new MySqlQueryValue
                    {
                        ["player_name"] = playerName,
                        ["counts"] = currentCount.ToString()
                    });
            }
            else
            {
                mySql.Table("cointhrow").Insert(new MySqlQueryValue
                {
                    ["player_steamid"] = playerSteamID,
                    ["player_name"] = playerName,
                    ["counts"] = "1"
                });
            }
        }
        [ConsoleCommand("css_hodmincou", "Throw a coin with the result being heads or tails")]
        public void OnCoinThrowCommand(CCSPlayerController? player, CommandInfo command)
        {
            if (player != null && player.PlayerPawn.Value != null)
            {
                if (!CanPlayerThrowCoin(player.SteamID.ToString()))
                {
                    player.PrintToChat($"You can throw the coin only once every {ChatColors.DarkRed}10 {ChatColors.Default}seconds!");
                    return;
                }

                Random random = new Random();
                bool coinResult = random.Next(2) == 0;
                string resultString = coinResult ? "Heads" : "Tails";
                Server.PrintToChatAll($"The player {ChatColors.Green}{player.PlayerName}{ChatColors.Default} threw the coin and the result is {ChatColors.Green}{resultString}{ChatColors.Default}.");
                UpdateLastCoinThrowTime(player.SteamID.ToString());
                AddOrUpdateCoinThrowInDatabase(player.SteamID.ToString(), player.PlayerName);
            }
        }
    }
}
