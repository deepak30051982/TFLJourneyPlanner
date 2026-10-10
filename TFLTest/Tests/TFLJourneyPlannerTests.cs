using Microsoft.Playwright;
using Microsoft.Playwright.MSTest;
using System.Text.RegularExpressions;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;
using static Microsoft.Playwright.Assertions;

namespace TFLTest.Tests
{
    [TestClass]
    public sealed class TFLJourneyPlannerTests //: PageTest
    {
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;
        private IPlaywright _playwright;

        [TestInitialize]
        public async Task Setup()
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

            //I am on the right page
            await Expect(_page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("journey"));

            //accept cookies
            var acceptCookiesButton = _page.GetByRole(AriaRole.Button, new() { Name = "Accept only essential cookies" });
            if (await acceptCookiesButton.IsVisibleAsync())
            {
                await acceptCookiesButton.ClickAsync();

            }
        }


        public async Task VerifyJourneyPlan()
        {
            var fromLocationInput = _page.GetByRole(AriaRole.Combobox, new() { Name = "From location" });
            var toLocationInput = _page.GetByRole(AriaRole.Combobox, new() { Name = "To location" });
            var planJourneyButton = _page.GetByRole(AriaRole.Button, new() { Name = "Plan my journey" });


            string from = "Leicester Square Underground Station";
            await fromLocationInput.WaitForAsync(new() { Timeout = 30000 });
            await fromLocationInput.FillAsync("Leicester Square");

            var suggestion1 = _page.GetByText(from, new() { Exact = true });
            await suggestion1.WaitForAsync(new() { Timeout = 30000 });
            await suggestion1.ClickAsync();


            string to = "Covent Garden Underground Station";
            await toLocationInput.WaitForAsync(new() { Timeout = 30000 });
            await toLocationInput.FillAsync("Garden");

            var suggestion2 = _page.GetByText(to, new() { Exact = true });
            await suggestion2.WaitForAsync(new() { Timeout = 30000 });
            await suggestion2.ClickAsync();



            await planJourneyButton.WaitForAsync(new() { Timeout = 30000 });
            await planJourneyButton.ClickAsync();

            var Option = _page.GetByRole(AriaRole.Heading, new() { Name = "Walking and cycling" });
            await Option.WaitForAsync(new() { Timeout = 30000 });
            Assert.IsTrue(await Option.IsVisibleAsync());

            await _page.GetByRole(AriaRole.Button, new() { Name = "Edit preferences" }).ClickAsync();
            await _page.GetByRole(AriaRole.Radio, new() { Name = "Routes with least walking" }).ClickAsync();
            await _page.GetByRole(AriaRole.Button, new() { Name = "Update journey" }).ClickAsync();

            // check if first option has a jorney time in the mins format

            var timeText = await _page.Locator("#option-1-heading .journey-time").InnerTextAsync();
            Assert.IsTrue(Regex.IsMatch(timeText.Trim(), @"^\d+\s*mins$"), $"Invalid time format: {timeText}");

        }


        public async Task VerifyLeastWalkingJourney()
        {
        }

        public async Task VerifyAccessInformation()
        {
        }

        [TestMethod]
        public async Task VerifyInvlidLocations()
        {
            var fromLocationInput = _page.GetByRole(AriaRole.Combobox, new() { Name = "From location" });
            var toLocationInput = _page.GetByRole(AriaRole.Combobox, new() { Name = "To location" });
            var planJourneyButton = _page.GetByRole(AriaRole.Button, new() { Name = "Plan my journey" });

            await fromLocationInput.WaitForAsync(new() { Timeout = 30000 });
            await fromLocationInput.FillAsync("XYZ1");
            await toLocationInput.FillAsync("XYZ2");

            await planJourneyButton.WaitForAsync(new() { Timeout = 30000 });
            await planJourneyButton.ClickAsync();

            var errorMessage = _page.Locator("li.field-validation-error");

            await Expect(errorMessage).ToContainTextAsync("can't find a journey");


        }

        [TestMethod]
        public async Task VerifyNoLocations()
        {
            var planJourneyButton = _page.GetByRole(AriaRole.Button, new() { Name = "Plan my journey" });
            await planJourneyButton.WaitForAsync(new() { Timeout = 30000 });
            await planJourneyButton.ClickAsync();

            //page is not redirected to results page
           Assert.DoesNotContain(_page.Url, "/results");


            //error messages
            var fromError = _page.Locator("#InputFrom-error");
            await Expect(fromError).ToContainTextAsync("The From field is required.");

            var toError = _page.Locator("#InputTo-error");
            await Expect(toError).ToContainTextAsync("The To field is required.");

        }
    }
}
