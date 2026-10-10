using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using static Microsoft.Playwright.Assertions;

namespace TFLTest
{
    [TestClass]
    public sealed class TFLJourneyPlannerTests //: PageTest
    {
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;
        private IPlaywright _playwright;

        [TestMethod]
        public async Task TestMethod1()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(
            new BrowserTypeLaunchOptions
            {
                Headless = false,
                Args = new[] { "--start-maximized" }
            });
            _context = await _browser.NewContextAsync(
                new BrowserNewContextOptions
                {
                    ViewportSize = null
                });
            _page = await _context.NewPageAsync();


            await _page.GotoAsync("https://tfl.gov.uk/plan-a-journey/?cid=plan-a-journey");

            await Expect(_page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("journey"));
        }
    }
}
