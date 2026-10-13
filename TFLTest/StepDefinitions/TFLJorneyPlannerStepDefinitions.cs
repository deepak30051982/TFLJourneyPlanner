using Microsoft.Playwright;
using Reqnroll;
using System.Text.RegularExpressions;
using TFL.JourneyPlanner.Tests.Pages;

namespace TFL.JourneyPlanner.Tests.StepDefinitions
{
    [Binding]
    public class TFLJorneyPlannerStepDefinitions
    {
        private JourneyPlannerPage _journeyPlannerPage;
        private JourneyResultsPage _journeyResultsPage;
        private IPage _page;
        private ScenarioContext _scenarioContext;

        public TFLJorneyPlannerStepDefinitions(
        ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
            _page = _scenarioContext.Get<IPage>("Page");
            _journeyPlannerPage = new JourneyPlannerPage(_page);
            _journeyResultsPage = new JourneyResultsPage(_page);
        }

        [Given("I am on the TfL Plan a journey page")]
        public async Task GivenIAmOnTheTfLPlanAJourneyPage()
        {
            await _journeyPlannerPage.GoToAsync("https://tfl.gov.uk/plan-a-journey/?cid=plan-a-journey");
            //I am on the right page
            //await Expect(_page).ToHaveTitleAsync(new System.Text.RegularExpressions.Regex("journey"));
           
        }

        [When("I plan a journey from {string} to {string}")]
        public async Task WhenIPlanAJourneyFromTo(
            string from,
            string to)
        {
            await _journeyPlannerPage.SelectFromLocationAsync("Leicester Square", from);

            await _journeyPlannerPage.SelectToLocationAsync("Covent Garden", to);

            await _journeyPlannerPage.ClickPlanJourneyAsync();
        }

        [Then("walking and cycling options should be displayed")]
        public async Task ThenWalkingAndCyclingOptionsShouldBeDisplayed()
        {
            bool visible = await _journeyResultsPage.IsWalkingAndCyclingVisibleAsync();

            Assert.IsTrue(visible);
        }

        [When("I choose routes with least walking")]
        public async Task WhenIChooseRoutesWithLeastWalking()
        {
            await _journeyResultsPage.SelectLeastWalkingRouteAsync();
        }

        [Then("the journey results should show a journey time")]
        public async Task ThenTheJourneyResultsShouldShowAJourneyTime()
        {
            var time =
                await _journeyResultsPage.GetJourneyTimeAsync() ?? "";

            var match = Regex.Match(
                time,
                @"(\d+)\s*mins",
                RegexOptions.IgnoreCase);

            Assert.IsTrue(
                match.Success,
                $"Invalid time format: {time}");
        }

        [When("I open journey details")]
        public async Task WhenIOpenJourneyDetails()
        {
            await _journeyResultsPage.OpenJourneyDetails();
        }

        [Then("complete access information should be available")]
        public async Task ThenCompleteAccessInformationShouldBeAvailable()
        {
            var accessInfo = await _journeyResultsPage.GetAccessInformationAsync();
            Assert.IsTrue(accessInfo.Any());
        }

        [When("I enter invalid journey locations")]
        public async Task WhenIEnterInvalidJourneyLocations()
        {
            await _journeyPlannerPage.EnterInvalidLocationsAsync(
                "Invalid From Location",
                "Invalid To Location");

            await _journeyPlannerPage.ClickPlanJourneyAsync();
        }

        [Then("the planner should not show any journey results")]
        public async Task ThenThePlannerShouldNotShowAnyJourneyResults()
        {
            var error =
                await _journeyResultsPage.GetErrorMessageAsync();

            StringAssert.Contains(
                error,
                "can't find");
        }

        [When("I submit the journey planner with no locations")]
        public async Task WhenISubmitTheJourneyPlannerWithNoLocations()
        {
            await _journeyPlannerPage.ClickPlanJourneyAsync();
        }

        [Then("journey planning should not proceed")]
        public void ThenJourneyPlanningShouldNotProceed()
        {
            Assert.DoesNotContain("/results", _page.Url);
        }

        [Then("an error message should be displayed")]
        public async Task ThenAnErrorMessageShouldBeDisplayed()
        {
            var fromError =
                await _journeyPlannerPage.GetFromRequiredErrorAsync();

            var toError =
                await _journeyPlannerPage.GetToRequiredErrorAsync();

            Assert.AreEqual(
                "The From field is required.",
                fromError);

            Assert.AreEqual(
                "The To field is required.",
                toError);
        }
    }
}