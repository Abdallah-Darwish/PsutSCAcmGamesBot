using Discord.Commands;
using Discord.WebSocket;
using System;
using System.Linq;
using System.Threading.Tasks;
using ParameterInfo = Discord.Commands.ParameterInfo;

namespace AcmGamesBot.Commands.Conditions
{
    /// <summary>
    /// Checks if the server has a category with supplied name that has text or voice channels
    /// </summary>
    public class ChannelCategoryAttribute : ParameterPreconditionAttribute
    {
        private readonly bool _text, _voice, _notEmpty;
        /// <param name="hasVoiceCahnnels">Should the category have voice channels.</param>
        /// <param name="hasTextChannels">Should the category have text channels.</param>
        /// <param name="notEmpty">Should the category have either text or voice channels.</param>
        public ChannelCategoryAttribute(bool hasVoiceCahnnels = true, bool hasTextChannels = false, bool notEmpty = true)
        {
            _voice = hasVoiceCahnnels;
            _text = hasTextChannels;
            _notEmpty = notEmpty || _voice || _text;
        }
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            string catName = value.ToString();
            var cat = (await context.Guild.GetChannelsAsync().ConfigureAwait(false))
            .OfType<SocketCategoryChannel>()
            .FirstOrDefault(c => c.Name.Equals(catName, StringComparison.OrdinalIgnoreCase));
            if (cat == null)
            {
                return PreconditionResult.FromError($"There is no channel category named {catName}");
            }
            if (cat.Channels.Count == 0 && _notEmpty)
            {
                return PreconditionResult.FromError($"Category {cat.Name} has no channels.");
            }
            if (_text && !cat.Channels.Any(c => c is SocketTextChannel))
            {
                return PreconditionResult.FromError($"Category {cat.Name} has no text channels.");
            }
            if (_voice && !cat.Channels.Any(c => c is SocketVoiceChannel))
            {
                return PreconditionResult.FromError($"Category {cat.Name} has no voice channels.");
            }
            return PreconditionResult.FromSuccess();
        }
    }
}
