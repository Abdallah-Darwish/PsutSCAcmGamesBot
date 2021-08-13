using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Text.Json;
using Serilog.Events;

namespace AcmGamesBot
{
    /// <summary>
    /// Container for program settings.
    /// </summary>
    public class Settings
    {
        public string Token { get; set; } = "";
        public string LogPath { get; set; } = "AcmBotLogs.txt";
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
        public string RequiredRole { get; set; } = "";
        private static readonly JsonSerializerOptions SerializerOptions = new()
        {
            WriteIndented = true
        };
        public static string FilePath { get; } = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AcmGamesBotSettings.json");

        public async Task Save()
        {
            using var fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
            await JsonSerializer.SerializeAsync(fileStream, this, SerializerOptions).ConfigureAwait(false);
        }
        public static async Task<Settings> Load()
        {
            try
            {
                using var fileStream = new FileStream(FilePath, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read);
                return await JsonSerializer.DeserializeAsync<Settings>(fileStream, SerializerOptions).ConfigureAwait(false);
            }
            catch
            {
                var settings = new Settings();
                await settings.Save().ConfigureAwait(false);
                return settings;
            }
        }
    }
}