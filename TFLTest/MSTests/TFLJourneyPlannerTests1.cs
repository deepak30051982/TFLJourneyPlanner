using Microsoft.Playwright;
using System.Text.RegularExpressions;
using TFL.JourneyPlanner.Core.BrowserFactory;
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

       
        public static async Task AssemblyInitialize(TestContext context)
        {
            await PlaywrightSession.StartAsync();

            _browser = PlaywrightSession.Browser;
        }

       
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



  
        public async Task VerifyJourneyPlan()
        {
            await PlanJourney();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            bool isWalkingAndCyclingVisible = await _journeyResultsPage.IsWalkingAndCyclingVisibleAsync();
            Assert.IsTrue(isWalkingAndCyclingVisible);

        }

 
        public async Task VerifyLeastWalkingJourney()
        {
            await PlanJourney();
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            await _journeyResultsPage.SelectLeastWalkingRouteAsync();

            // check if first option has a jorney time in the mins format
            var timeText = await _journeyResultsPage.GetJourneyTimeAsync()?? "";
            //Assert.IsTrue(Regex.IsMatch(timeText.Trim(), @"^\d+\s*mins$"), $"Invalid time format: {timeText}");

            var match = Regex.Match(timeText, @"(\d+)\s*mins", RegexOptions.IgnoreCase);
            Assert.IsTrue(match.Success, $"Invalid time format: {timeText}");

        }


        public async Task VerifyAccessInformation()
        {
        }


        public async Task VerifyInvlidLocations()
        {
            await _journeyPlannerPage.EnterInvalidLocationsAsync("Invalid From Location", "Invalid To Location");
            await _journeyPlannerPage.ClickPlanJourneyAsync();

            var errorMessage = await _journeyResultsPage.GetErrorMessageAsync();
            Assert.Contains("can't find", errorMessage);



        }


        public async Task VerifyNoLocations()
        {
            await _journeyPlannerPage.ClickPlanJourneyAsync();

            var fromErrorText = await _journeyPlannerPage.GetFromRequiredErrorAsync();
            var toErrorText = await _journeyPlannerPage.GetToRequiredErrorAsync();

            Assert.AreEqual("The From field is required.", fromErrorText);
            Assert.AreEqual("The To field is required.", toErrorText);
        }

        private async Task PlanJourney()
        {
            await _journeyPlannerPage.SelectFromLocationAsync("Leicester Square", "Leicester Square Underground Station");
            await _journeyPlannerPage.SelectToLocationAsync("Garden", "Covent Garden Underground Station");
            await _journeyPlannerPage.ClickPlanJourneyAsync();
        }
    }
}
