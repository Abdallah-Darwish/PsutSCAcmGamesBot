using Discord.Commands;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace AcmGamesBot.Commands.Conditions
{
    public class CheckRoleAttribute : PreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context, CommandInfo command, IServiceProvider services)
        {
            var st = services.GetService<Settings>()!;
            var roleName = st.RequiredRole;
            if (string.IsNullOrWhiteSpace(roleName)) { return PreconditionResult.FromSuccess(); }

            var logger = services.GetService<ILogger>()!;

            var user = await context.Guild.GetUserAsync(context.User.Id).ConfigureAwait(false);
            var role = context.Guild.Roles.FirstOrDefault(r => r.Name.Equals(roleName, StringComparison.OrdinalIgnoreCase));

            if (role != null)
            {
                if (user.RoleIds.Contains(role.Id)) { return PreconditionResult.FromSuccess(); }
            }
            else
            {
                logger.Verbose("Server {server} doesn't have a role named \"{role}\".", context.Guild.Name, roleName);
            }

            return PreconditionResult.FromError($"User {user.Username} doesn't have role \"{roleName}\".");
        }
    }
}
