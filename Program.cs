using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using AcmGamesBot.Commands;
using Discord;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
namespace AcmGamesBot
{
    public class Program
    {
        public static IServiceProvider DIContainer { get; private set; }
        private static AcmBot? _bot;
        public static async Task<int> Main()
        {
            var st = await Settings.Load().ConfigureAwait(false);

            var loggerConfig = new LoggerConfiguration()
                .MinimumLevel.Is(st.LogLevel)
                .WriteTo.Console();
            bool validLogPath = true;
            try
            {
                using var fs = new FileStream(st.LogPath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            }
            catch { validLogPath = false; }
            if (validLogPath)
            {
                loggerConfig.WriteTo.File(st.LogPath, buffered: false);
            }

            var logger = loggerConfig.CreateLogger();
            if (validLogPath)
            {
                logger.Information("Also logging to {path}", st.LogPath);
            }
            else
            {
                logger.Warning("Invalid logs file path.");
            }
            var sc = new ServiceCollection();

            sc.AddSingleton<ILogger>(logger)
            .AddTransient<AcmBot>()
            .AddTransient<MessageHandler>()
            .AddSingleton(st);

            var x = new Emoji("\u2764");

            DIContainer = sc.BuildServiceProvider();

            try
            {
                TokenUtils.ValidateToken(TokenType.Bot, st.Token);
            }
            catch
            {
                logger.Fatal("Invalid token \"{token}\", please update it and restart the program.", st.Token);
                Console.ReadLine();
                return 1;
            }
            try
            {
                Console.CancelKeyPress += StopBot;
                _bot = await AcmBot.Create(st.Token).ConfigureAwait(false);
                await Task.Delay(-1).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                logger.Fatal("{@ex}", ex);
            }
            logger.Dispose();
            return 0;
        }

        private static void StopBot(object? sender, ConsoleCancelEventArgs e)
        {
            _bot?.DisposeAsync().AsTask().Wait();
            Thread.Sleep(TimeSpan.FromSeconds(3));
        }
    }
}
