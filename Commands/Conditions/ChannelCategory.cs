using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using ParameterInfo = Discord.Commands.ParameterInfo;

namespace AcmGamesBot.Commands.Conditions
{
    public class ChannelCategoryAttribute : ParameterPreconditionAttribute
    {
        private readonly bool _text, _voice, _notEmpty;
        public ChannelCategoryAttribute(bool hasVoiceCahnnels = true, bool hasTextChannels = false, bool notEmpty = true)
        {
            _voice = hasVoiceCahnnels;
            _text = hasTextChannels;
            _notEmpty = notEmpty || _voice || _text;
        }
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            string catName = value.ToString();
            var cats = (await context.Guild.GetChannelsAsync().ConfigureAwait(false)).OfType<SocketCategoryChannel>().ToArray();
            var cat = Array.Find(cats, c => c.Name.Equals(catName, StringComparison.OrdinalIgnoreCase));
            if (cat == null)
            {
                return PreconditionResult.FromError($"There is no channel category nammed {catName}");
            }
            if (cat.Channels.Count == 0 && _notEmpty)
            {
                return PreconditionResult.FromError($"Category {cat.Name} has no channels.");
            }
            if (!cat.Channels.Any(c => c is SocketTextChannel) && _text)
            {
                return PreconditionResult.FromError($"Category {cat.Name} has no text channels.");
            }
            if (!cat.Channels.Any(c => c is SocketVoiceChannel) && _voice)
            {
                return PreconditionResult.FromError($"Category {cat.Name} has no voice channels.");
            }
            return PreconditionResult.FromSuccess();
        }
    }
}
