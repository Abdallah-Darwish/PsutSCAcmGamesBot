using Discord;
using Serilog.Events;

namespace AcmGamesBot
{
    internal static class Extensions
    {
       public static LogEventLevel ToSerilog(this LogSeverity s) => (LogEventLevel)(5 - (int)s);
    }
}
