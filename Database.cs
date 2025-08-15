using Dapper;
using MySqlConnector;
using System.Data;

namespace CoinThrow
{
    public class Database(string host, int port, string database, string user, string password)
    {
        private readonly string _connectionString = $"Server={host};Port={port};Database={database};User={user};Password={password};";
        private IDbConnection? _db;

        public void Initialize()
        {
            try
            {
                _db = new MySqlConnection(_connectionString);
                _db.Execute(@"
                    CREATE TABLE IF NOT EXISTS `cointhrow` (
                        `id` INT AUTO_INCREMENT PRIMARY KEY,
                        `player_steamid` VARCHAR(32) NOT NULL UNIQUE,
                        `player_name` VARCHAR(32) NOT NULL,
                        `counts` INT NOT NULL DEFAULT 0
                    );
                ");

                Console.WriteLine("[CoinThrow] Connected to database successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[CoinThrow] Database connection failed: {ex.Message}");
            }
        }

        public int IncrementPlayerThrows(string steamId, string playerName)
        {
            if (_db == null) throw new InvalidOperationException("Database not initialized.");

            _db.Execute(@"
                INSERT INTO cointhrow (player_steamid, player_name, counts)
                VALUES (@SteamID, @Name, 1)
                ON DUPLICATE KEY UPDATE
                    player_name = VALUES(player_name),
                    counts = counts + 1;
            ", new { SteamID = steamId, Name = playerName });

            return _db.QuerySingle<int>(
                "SELECT counts FROM cointhrow WHERE player_steamid = @SteamID",
                new { SteamID = steamId }
            );
        }
    }
}