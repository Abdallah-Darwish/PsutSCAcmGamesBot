using Discord;
using Discord.Commands;
using AcmGamesBot.Commands.Conditions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Serilog;

namespace AcmGamesBot.Commands
{
    [Group("acm")]
    [RequireBotPermission(GuildPermission.MentionEveryone | GuildPermission.MoveMembers | GuildPermission.SendMessages | GuildPermission.ViewChannel)]
    [CheckRole]
    public class AcmModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _commandService;
        private readonly ILogger _logger;
        public AcmModule(ILogger lgr, CommandService coms)
        {
            _commandService = coms;
            _logger = lgr;
        }
        [Command("Help")]
        [Summary("Prints information about all available commands in this bot.")]
        public async Task Help()
        {
            var commands = _commandService.Commands.ToArray();
            var embedBuilder = new EmbedBuilder();
            embedBuilder.WithColor(Color.Blue).WithTitle("Available Commands").WithFooter("Texts with spaces must be enclosed within double quotes.");
            var cmdSb = new StringBuilder();
            foreach (CommandInfo command in commands)
            {
                cmdSb.Clear();
                cmdSb.AppendLine(command.Summary).AppendLine();
                if (!string.IsNullOrWhiteSpace(command.Remarks))
                {
                    cmdSb.Append("Remarks: ").AppendLine(command.Remarks).AppendLine();
                }
                if (command.Parameters.Count != 0)
                {
                    cmdSb.AppendLine("Parameters:");
                    foreach (var par in command.Parameters)
                    {
                        cmdSb.Append(par.Name).Append(par.IsOptional ? "[Optional]" : "").Append('<').Append(par.Type.Name).Append(">: ").AppendLine(par.Summary);
                    }
                    cmdSb.AppendLine();
                }

                embedBuilder.AddField(command.Name, cmdSb.ToString());
            }
            await ReplyAsync(embed: embedBuilder.Build()).ConfigureAwait(false);
            _logger.Information("User {username} in {server} asked for help, replied with {count} commands.",
            Context.User.Username, Context.Guild.Name, commands.Length);
        }

        [Command("Ping")]
        [Summary("Check if I am alive")]
        public async Task Ping()
        {
            var username = Context.User.Username;
            _logger.Information("{username} from {server} pinged me.", username, Context.Guild.Name);
            await ReplyAsync($"Pong {username}").ConfigureAwait(false);
        }
        [Command(nameof(Partition), RunMode = RunMode.Async)]
        [Summary("Divides reactors into equal groups between channels after a timeout.")]
        public async Task Partition(
            [Summary("Name of the game you want to play.")]
            string gameName,
            [ChannelCategory]
            [Summary("Category of the channels to divide players among them.")]
            string channelsCategory,
            [InRange(1)]
            [Summary("Size of each players group")]
            int groupSize,
            [InRange(1)]
            [Summary("Time to wait until all players react to the message.")]
            int timeout = 120)
        {
            _logger.Information("User {username} from {server} wants to play {gameName}.", Context.User.Username, Context.Guild.Name, gameName);
            var msg = await ReplyAsync($"{Context.Guild.EveryoneRole.Mention} React to this message if you want to play {gameName} (and you must be in a voice channel so I can move you), you will be partitioned in {timeout} seconds.").ConfigureAwait(false);
            await Task.Delay(timeout * 1000).ConfigureAwait(false);
            msg = (await msg.Channel.GetMessageAsync(msg.Id).ConfigureAwait(false) as IUserMessage)!;
            var checkedUsers = new HashSet<ulong>();
            var players = new List<IGuildUser>();
            foreach (var react in msg.Reactions)
            {
                foreach (var p in await msg.GetReactionUsersAsync(react.Key, int.MaxValue).FlattenAsync().ConfigureAwait(false))
                {
                    if (!checkedUsers.Add(p.Id)) { continue; }
                    var user = Context.Guild.GetUser(p.Id);
                    if (user.VoiceChannel == null)
                    {
                        _logger.Verbose("User {username} from server {guildName} reacted to my message but he is not in any channel.", user.Username, Context.Guild.Name);
                        continue;
                    }
                    players.Add(user);
                }
            }

            if (players.Count == 0)
            {
                _logger.Verbose("No one wants to play {gameName} with {username} in {serverName}.", gameName, Context.User.Username, Context.Guild.Name);
                await ReplyAsync($"No one wants to play {gameName} \ud83d\ude41").ConfigureAwait(false);
                return;
            }

            var rand = new Random();
            int playersLen = players.Count;
            int k;
            IGuildUser tmp;
            while (playersLen > 1)
            {
                k = rand.Next(playersLen--);
                tmp = players[playersLen];
                players[playersLen] = players[k];
                players[k] = tmp;
            }

            await ReplyAsync($"{players.Count} players want to play {gameName}").ConfigureAwait(false);

            var channelsCat = Context.Guild.CategoryChannels.First(c => c.Name.Equals(channelsCategory, StringComparison.OrdinalIgnoreCase))!;
            var voiceChannels = channelsCat.Channels.OfType<IVoiceChannel>().OrderBy(c => c.Position).ToArray()!;
            var lastCahannel = voiceChannels.Last();
            foreach (var p in players)
            {
                if (p.VoiceChannel == lastCahannel) { continue; }
                _logger.Verbose("Moving {user} in {server} from channel {srcChannel} to {dstChannel}.", p.Username, Context.Guild.Name, p.VoiceChannel.Name, lastCahannel.Name);
                await p.ModifyAsync(prop => prop.Channel = new Optional<IVoiceChannel>(lastCahannel)).ConfigureAwait(false);
            }

            for (int i = 0; i < voiceChannels.Length - 1; i++)
            {
                foreach (var p in players.Skip(i * groupSize).Take(groupSize))
                {
                    _logger.Verbose("Moving {user} in {server} from channel {srcChannel} to {dstChannel}.", p.Username, Context.Guild.Name, p.VoiceChannel.Name, voiceChannels[i].Name);
                    await p.ModifyAsync(prop => prop.Channel = new Optional<IVoiceChannel>(voiceChannels[i])).ConfigureAwait(false);
                }
            }
            _logger.Information("Partitioned {playersCount} in {server} into {channelsCount} channels for a game of {gameName}.",
            players.Count, Context.Guild.Name, Math.Min(voiceChannels.Length, Math.Ceiling(players.Count / (double)voiceChannels.Length)), gameName);
            await ReplyAsync("Enjoy !").ConfigureAwait(false);
        }
    }
}
