using Discord.Commands;
using System;
using System.Threading.Tasks;
using ParameterInfo = Discord.Commands.ParameterInfo;

namespace AcmGamesBot.Commands.Conditions
{
    public class InRangeAttribute : ParameterPreconditionAttribute
    {
        private readonly int _min, _max;

        public InRangeAttribute(int min, int max = int.MaxValue)
        {
            if (min > max) { throw new ArgumentOutOfRangeException(nameof(min), min, "Paramter min must be <= max."); }
            _min = min;
            _max = max;
        }
        public override Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, ParameterInfo parameter, object value, IServiceProvider services)
        {
            PreconditionResult result;
            if (!int.TryParse(value.ToString(), out var intValue))
            {
                result = PreconditionResult.FromError($"Value: \"{value}\" is not a valid integer.");
            }
            else if (intValue < _min || intValue > _max)
            {
                result = PreconditionResult.FromError($"{value} is not in range [{_min}, {_max}].");
            }
            else { result = PreconditionResult.FromSuccess(); }
            return Task.FromResult(result!);
        }
    }
}
