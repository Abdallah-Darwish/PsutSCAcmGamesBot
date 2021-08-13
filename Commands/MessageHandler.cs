using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Serilog;
using System;
using System.Reflection;
using System.Threading.Tasks;

namespace AcmGamesBot.Commands
{
    /// <summary>
    /// Will read all of the servers messages and invoke commands.
    /// </summary>
    public class MessageHandler : IDisposable
    {
        private DiscordSocketClient? _client;
        private CommandService? _commands;
        private IServiceProvider? _diContainer;
        private bool _disposed;
        private readonly ILogger _logger;
        public MessageHandler(ILogger logger, IServiceProvider diContainer)
        {
            _logger = logger;
            _diContainer = diContainer;
        }
        /// <summary>
        /// Adds a message listener to read servers messages and respond to commands.
        /// </summary>
        private async Task InstallCommands(DiscordSocketClient client)
        {
            _client = client;

            _commands = new CommandService();
            var mods = await _commands.AddModulesAsync(Assembly.GetExecutingAssembly(), _diContainer).ConfigureAwait(false);
            foreach (var mod in mods)
            {
                _logger.Information("Installed {moduleName} on {clientName}.", mod.Name, _client.CurrentUser?.Username ?? "[UNKNOWN USER]");
            }
            _client.MessageReceived += HandleCommandAsync;
            _commands.CommandExecuted += PostCommandExecution;
        }
        /// <summary>
        /// Creates a <see cref="MessageHandler"/> based on <paramref name="client"/> to read messages and execute commands.
        /// </summary>
        public async static Task<MessageHandler> Create(DiscordSocketClient client)
        {
            var handler = Program.DIContainer.GetService<MessageHandler>()!;
            await handler.InstallCommands(client).ConfigureAwait(false);
            return handler;
        }

        /// <summary>
        /// Logs command execution result.
        /// </summary>
        private async Task PostCommandExecution(Optional<CommandInfo> command, ICommandContext context, IResult result)
        {
            if (!result.IsSuccess)
            {
                _logger.Error("Messgae \"{msg}\" from user {user} in {server} caused the following error:\n{error}: {reason}", context.Message.Content, context.User.Username, context.Guild.Name, result.Error, result.ErrorReason);
                await context.Channel.SendMessageAsync(result.ErrorReason).ConfigureAwait(false);
            }
            else
            {
                _logger.Verbose("User {user} in {server} executed \"{cmd}\" successfully.", context.User.Username, context.Guild.Name, context.Message.Content);
            }
        }
        /// <summary>
        /// Listen to all messages and execute any commands.
        /// </summary>
        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            if (messageParam is not SocketUserMessage message) return;
            int argPos = 0;

            if (!message.HasCharPrefix('!', ref argPos) && !message.HasMentionPrefix(_client!.CurrentUser, ref argPos))
            {
                return;
            }
            if (message.Author.IsBot)
            {
                _logger.Information("{botName} bot in {server} tried to interact with me.", message.Author.Username, _client!.GetGuild(message.Reference.GuildId.Value).Name);
            }
            var context = new SocketCommandContext(_client, message);

            await _commands!.ExecuteAsync(
                context: context,
                argPos: argPos,
                services: _diContainer).ConfigureAwait(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _client != null)
                {
                    _logger.Information("Uninstalled all modules from {name}.", _client.CurrentUser?.Username ?? "[UNKNOWN]");
                    _client.MessageReceived -= HandleCommandAsync;
                }
                _diContainer = null;
                _client = null;
                _commands = null;
                _disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
