*Prerequisite:
-.NET Runtime 5.0.0, you can find it at https://dotnet.microsoft.com/download/dotnet/5.0

*How to run the program:
1- Create an application at https://discord.com/developers .
2- Make a bot for your app.
3- Go to OAuth2 and from "OAuth2 URL Generator" select bot scope with the following permissions:
    A- Mention Everyone to use @everyone 
    B- Move Members to move players between channels
    C- SendMessages
    D- ViewChannels
4- Open the generated link and add the bot to your server.
5- At https://discord.com/developers/applications open your application bot and copy its token.
6- Start the AcmGamesBot.exe for the first time to create a settings file.
7- Write your token in AcmBotSetting.json .
9- Test if the bot is working by typing "!acm help" in your server.
8- ...profit!

*AcmBotSetting.json:
1- "LogPath" is the path to logs file, if it's invalid path then no file will be created.
2- "LogLevel" is the minimum level of logs to output and it can have the following values:
    A- 0, Verbose, Anything and everything you might want to know about a running block of code.
    B- 1, Debug, Internal system events that aren't necessarily observable from the outside.
    C- 2, Information(recommended & default value), The lifeblood of operational intelligence - things happen.
    D- 3, Warning, Service is degraded or endangered.
    E- 4, Error, Functionality is unavailable, invariants are broken or data is lost.
    F- 5, Fatal, If you have a pager, it goes off when one of these occurs.
All values above are copied from "Serilog.Events.LogEventLevel" .
3- "Token" is your bot token so the program can connect to it.
4- "RequiredRole" is name of the role users should have to interact with this bot, if its null or empty then it will be ignored.
