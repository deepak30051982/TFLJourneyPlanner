using Microsoft.Playwright;

namespace TFL.JourneyPlanner.Core.BrowserFactory
{

    public static class PlaywrightSession
    {
        public static IPlaywright Playwright { get; private set; } = null!;
        public static IBrowser Browser { get; private set; } = null!;

        public static async Task StartAsync()
        {
            Playwright = await Microsoft.Playwright.Playwright.CreateAsync();

            Browser = await Playwright.Chromium.LaunchAsync(
                new BrowserTypeLaunchOptions
                {
                    Headless = true,
                    Args = new List<string> { "--start-maximized" }
                });
        }

        public static async Task<IBrowserContext> CreateContextAsync()
        {
            return await Browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    ViewportSize = ViewportSize.NoViewport
                });
        }

        public static async Task StopAsync()
        {
            await Browser.CloseAsync();
            Playwright.Dispose();
        }
    }

}