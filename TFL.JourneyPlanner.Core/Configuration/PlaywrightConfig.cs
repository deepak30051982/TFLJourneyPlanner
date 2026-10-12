namespace TFL.JourneyPlanner.Core.Configuration
{
    public sealed class PlaywrightConfig
    {
        public string BaseUrl { get; init; }
        public string PlanJourneyUrl { get; init; }
        public int DefaultTimeoutSeconds { get; init; } = 30;
        public bool CaptureTraces { get; init; } = true;
        public string? BrowserType { get; init; }

        public static PlaywrightConfig Current { get; } = PlaywrightConfigOptions.LoadFromJson();
    }

    public class PlaywrightConfigOptions
    {
        public static PlaywrightConfig LoadFromJson()
        {
            var directory = AppContext.BaseDirectory;
            var configFilePath = System.IO.Path.Combine(directory, "playwrightconfig.json");

            if (!System.IO.File.Exists(configFilePath))
            {
                throw new System.IO.FileNotFoundException($"Configuration file not found: {configFilePath}");
            }

            var configContent = System.IO.File.ReadAllText(configFilePath);
            return System.Text.Json.JsonSerializer.Deserialize<PlaywrightConfig>(configContent) ??
                throw new System.Text.Json.JsonException("Failed to deserialize configuration");
        }
    }
}
