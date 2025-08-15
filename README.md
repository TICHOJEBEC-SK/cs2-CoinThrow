<h1 align="center">
  CS2 CoinThrow (JailBreak)
</h1>

<p align="center">
<i>If you like this plugin, please consider <a href="https://paypal.com/paypalme/playpointsk">donating</a> 💸 to help it improve!</i>
</p>

<p align="center">
<a href="https://www.paypal.com/paypalme/mleaguecz">
  <img src="https://img.shields.io/badge/support-PayPal-blue?logo=PayPal&style=flat-square&label=Donate"/>
</a>
</p>

#### ABOUT THE PLUGIN

This plugin allows wardens/guards to toss a coin, with the result being either heads or tails.  
It is fully connected to a MySQL database via **Dapper**, so you can track how many times each player (warden) has thrown the coin.  

**Use case:** Prisoners can guess the outcome, and the warden decides the punishment for wrong guesses.  
<img src="https://i.ibb.co/nwvGh0Y/image.png" alt="CoinThrow plugin screenshot"/>

**Features:**
- Coin toss result: Heads or Tails.
- Database integration with MySQL.
- Counts total throws per player.
- 10-second cooldown between coin tosses.

## 💡 Contact
For questions or support, contact me via Discord: **tichotm**

## 🛠️ Installation Steps

**Required:**
- [CounterStrikeSharp](https://github.com/roflmuffin/CounterStrikeSharp)

**Installation:**
1. Download the plugin ZIP file and extract it.
2. Locate the `Gameserver` folder inside the extracted files.
3. Copy all files from `Gameserver` to `/game/csgo/addons/counterstrikesharp/plugins` on your server.
4. Restart the server or change the map to apply changes.
5. Edit the generated `Config.json` to set the correct database connection (`DBHost`, `DBUser`, `DBPassword`, `DBDatabase`, `DBPort`).

## 🙇 Sponsors
- Be the first to support this project!

## 🙏 Support
<p align="left">
<a href="https://paypal.com/paypalme/playpointsk"><img src="https://ionicabizau.github.io/badges/paypal.svg"></a>
</p>
