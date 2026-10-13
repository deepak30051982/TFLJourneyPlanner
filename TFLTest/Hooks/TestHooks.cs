using Microsoft.Playwright;
using Reqnroll;
using TFL.JourneyPlanner.Core.BrowserFactory;

namespace TFL.JourneyPlanner.Tests.Hooks
{
    [Binding]
    public class TestHooks
    {
        private readonly ScenarioContext _scenarioContext;

        public TestHooks(
            ScenarioContext scenarioContext)
        {
            _scenarioContext = scenarioContext;
        }

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            var context = await PlaywrightSession.CreateContextAsync();

            var page = await context.NewPageAsync();

            _scenarioContext["Context"] = context;
            _scenarioContext["Page"] = page;
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            if (_scenarioContext.TryGetValue(
                    "Context",
                    out IBrowserContext? context))
            {
                await context.CloseAsync();
            }
        }

        [BeforeTestRun]
        public static async Task BeforeTestRun()
        {
            await PlaywrightSession.StartAsync();
        }

        [AfterTestRun]
        public static async Task AfterTestRun()
        {
            await PlaywrightSession.StopAsync();
        }
    }
}
