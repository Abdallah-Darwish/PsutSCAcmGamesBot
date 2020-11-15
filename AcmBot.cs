using System;
using System.Threading.Tasks;
using AcmGamesBot.Commands;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace AcmGamesBot
{
    public class AcmBot : IAsyncDisposable, IDisposable
    {
        private DiscordSocketClient? _client;
        private MessageHandler? _handler;
        private bool _disposed;
        private readonly ILogger _logger;
        public AcmBot(ILogger logger)
        {
            _logger = logger;
        }
        private async Task Start(string token)
        {
            //printing token ! smort
            _logger.Information("Starting the bot with {token}", token);
            var config = new DiscordSocketConfig
            {
                DefaultRetryMode = RetryMode.AlwaysRetry,
                MessageCacheSize = 1000,
                LogLevel = LogSeverity.Verbose
            };
            _client = new DiscordSocketClient(config);
            _client.Log += Log;
            _client.Connected += InitHandler;
            _logger.Information("Connecting to server.");
            await _client.StartAsync().ConfigureAwait(false);
            _logger.Information("Logging in.");
            await _client.LoginAsync(TokenType.Bot, token).ConfigureAwait(false);
        }

        private async Task InitHandler()
        {
            if (_handler != null) { return; }
            _handler = await MessageHandler.Create(_client!).ConfigureAwait(false);
        }

        public async static Task<AcmBot> Create(string token)
        {
            var bot = Program.DIContainer.GetService<AcmBot>()!;
            await bot.Start(token).ConfigureAwait(false);
            return bot;
        }
        private Task Log(LogMessage log)
        {
            if (log.Exception != null)
            {
                _logger.Write(log.Severity.ToSerilog(), "{message}\n{@ex}", log.Message, log.Exception);
            }
            else { _logger.Write(log.Severity.ToSerilog(), "{message}", log.Message); }
            return Task.CompletedTask;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) { return; }
            _disposed = true;
            _handler?.Dispose();
            if (_client != null)
            {
                if (_client.CurrentUser != null)
                {
                    _logger.Information("Turning off {name} bot.", _client.CurrentUser.Username);
                }
                await _client.LogoutAsync().ConfigureAwait(false);
                await _client.StopAsync().ConfigureAwait(false);
                _client.Log -= Log;
                _client.Connected -= InitHandler;
                _client.Dispose();
            }
            _handler = null;
            _client = null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    DisposeAsync().AsTask().Wait();
                }

                _handler = null;
                _client = null;
                _disposed = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
