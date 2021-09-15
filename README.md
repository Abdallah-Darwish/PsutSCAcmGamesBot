# PSUT ACM Students Club Games Bot

## Problem
We used to host board games night every Thursday at the university before Covid-19 to introduce members to each other, but during Covid-19 we couldn't because most of the university activities were online.

So we decided to host them online with games like ([Skribbl](https://skribbl.io/), [AmongUS](https://innersloth.com/gameAmongUs.php)) and talk over [Discord](https://discord.com/), still we needed a way to introduce new members to each other and this were the bot came in.

## Solution
The bot will send a message in the server and after a timeout it will divide the interactors to this message randomly into voice channels to play the game.

## Prerequisites
- [Dotnet 5 Sdk](https://dotnet.microsoft.com/download/dotnet/5.0).


## How to run the program
- Create an application at https://discord.com/developers .
- Make a bot for your app.
- Go to OAuth2 and from "OAuth2 URL Generator" and select bot scope with the following permissions:
    - Mention Everyone to use @everyone 
    - Move Members to move players between channels
    - SendMessages
    - ViewChannels
- Open the generated link and add the bot to your server.
- At https://discord.com/developers/applications open your application bot and copy its token.
- Start the AcmGamesBot.exe for the first time to create a settings file.
- Write your token in AcmBotSetting.json .
- Test if the bot is working by typing "!acm help" in your server.
- ...profit!

## Commands 
You can type `!acm help` after adding the bot to your server to see its commands.

## AcmBotSetting.json
- "LogPath" is the path to logs file, if it's invalid path then no file will be created.
- "LogLevel" is the minimum level of logs to output and it can have the following values:
    - 0, Verbose, Anything and everything you might want to know about a running block of code.
    - 1, Debug, Internal system events that aren't necessarily observable from the outside.
    - 2, Information(recommended & default value), The lifeblood of operational intelligence - things happen.
    - 3, Warning, Service is degraded or endangered.
    - 4, Error, Functionality is unavailable, invariants are broken or data is lost.
    - 5, Fatal, If you have a pager, it goes off when one of these occurs.
All values above are copied from "Serilog.Events.LogEventLevel" .
- "Token" is your bot token so the program can connect to it.
- "RequiredRole" is name of the role users should have to interact with this bot, if its null or empty then it will be ignored.

## Screenshots
![Help](https://github.com/Abdallah-Darwish/PsutSCAcmGamesBot/raw/main/Screenshots/Help.png)
![Ping](https://github.com/Abdallah-Darwish/PsutSCAcmGamesBot/raw/main/Screenshots/Ping.png)
![Unkown Command](https://github.com/Abdallah-Darwish/PsutSCAcmGamesBot/raw/main/Screenshots/UnknownCommand.png)
![BeforePartitioning](https://github.com/Abdallah-Darwish/PsutSCAcmGamesBot/raw/main/Screenshots/BeforePartitioning.png)
![AfterPartitioning](https://github.com/Abdallah-Darwish/PsutSCAcmGamesBot/raw/main/Screenshots/AfterPartitioning.png)
![Logs](https://github.com/Abdallah-Darwish/PsutSCAcmGamesBot/raw/main/Screenshots/Logs.png)
