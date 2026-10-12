using Microsoft.Playwright;
using System.Text.RegularExpressions;
using TFL.JourneyPlanner.Core.BrowserFactory.TFL.JourneyPlanner.MSTests.Core;
using TFL.JourneyPlanner.Tests.Pages;
using static Microsoft.Playwright.Assertions;

namespace TFL.JourneyPlanner.MSTests.Tests
{
    [TestClass]
    public sealed class TFLJourneyPlannerTests1 //: PageTest
    {
        private static IBrowser _browser = null!;
        private IBrowserContext _context = null!;
        private IPage _page = null!;

        private JourneyPlannerPage _journeyPlannerPage = null!;
        private JourneyResultsPage _journeyResultsPage = null!;

        [AssemblyInitialize]
        public static async Task AssemblyInitialize(TestContext context)
        {
            await PlaywrightSession.StartAsync();

            _browser = PlaywrightSession.Browser;
        }

        [AssemblyCleanup]
        public static async Task AssemblyCleanup()
        {
            await PlaywrightSession.StopAsync();
        }

        [TestInitialize]
        public async Task Setup()
        {
            _context = await PlaywrightSession.CreateContextAsync();
            _page = await _context.NewPageAsync();

            _journeyPlannerPage = new JourneyPlannerPage(_page);
            _journeyResultsPage = new JourneyResultsPage(_page);

            await _page.GotoAsync("https://tfl.gov.uk/plan-a-journey/?cid=plan-a-journey");

            //I am on the right page
            await Expect(_page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("journey"));

            //accept cookies
            var acceptCookiesButton = _page.GetByRole(AriaRole.Button, new() { Name = "Accept all cookies" });
            if (await acceptCookiesButton.IsVisibleAsync())
            {
                await acceptCookiesButton.ClickAsync();

                //added this because after accepting for cookies the page has a black overlay and the page is not clickable, so we need to reload the page to remove the overlay
                await _page.GotoAsync("https://tfl.gov.uk/plan-a-journey/?cid=plan-a-journey");

            }
        }



        [TestMethod]
        public async Task VerifyJourneyPlan()
        {
            await PlanJourney();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            bool isWalkingAndCyclingVisible = await _journeyResultsPage.IsWalkingAndCyclingVisible();
            Assert.IsTrue(isWalkingAndCyclingVisible);

        }

        [TestMethod]
        public async Task VerifyLeastWalkingJourney()
        {
            await PlanJourney();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _journeyResultsPage.SelectLeastWalkingRoute();

            // check if first option has a jorney time in the mins format
            var timeText = await _journeyResultsPage.GetJourneyTime();
            Assert.IsTrue(Regex.IsMatch(timeText.Trim(), @"^\d+\s*mins$"), $"Invalid time format: {timeText}");

        }


        public async Task VerifyAccessInformation()
        {
        }


        public async Task VerifyInvlidLocations()
        {
            await _journeyPlannerPage.EnterInvalidLocations("Invalid From Location", "Invalid To Location");
            await _journeyPlannerPage.ClickPlanJourney();

            var errorMessage = await _journeyResultsPage.GetErrorMessage();
            Assert.Contains("can't find", errorMessage);



        }


        public async Task VerifyNoLocations()
        {
            await _journeyPlannerPage.ClickPlanJourney();

            var fromErrorText = await _journeyPlannerPage.GetFromRequiredError();
            var toErrorText = await _journeyPlannerPage.GetToRequiredError();

            Assert.AreEqual("The From field is required.", fromErrorText);
            Assert.AreEqual("The To field is required.", toErrorText);
        }

        private async Task PlanJourney()
        {
            await _journeyPlannerPage.SelectFromLocation("Leicester Square", "Leicester Square Underground Station");
            await _journeyPlannerPage.SelectToLocation("Garden", "Covent Garden Underground Station");
            await _journeyPlannerPage.ClickPlanJourney();
        }
    }
}
