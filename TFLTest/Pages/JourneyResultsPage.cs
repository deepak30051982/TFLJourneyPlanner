using Microsoft.Playwright;

namespace TFL.JourneyPlanner.Tests.Pages
{
    public class JourneyResultsPage
    {
        private readonly IPage _page;

        public JourneyResultsPage(IPage page)
        {
            _page = page;
        }

        //locators

        private ILocator WalkingAndCyclingHeading => _page.GetByRole(AriaRole.Heading, new() { Name = "Walking and cycling" });
        private ILocator EditPreferencesButton => _page.GetByRole(AriaRole.Button, new() { Name = "Edit preferences" });
        // private ILocator LeastWalkingRadio => _page.GetByRole(AriaRole.Radio, new() { Name = "Routes with least walking" });
        // private ILocator LeastWalkingRadio => _page.Locator("#jp-search-form").Locator("#JourneyPreference_2");
        private ILocator LeastWalkingRadio => _page.Locator("label[for='JourneyPreference_2']");

        private ILocator UpdateJourneyButton => _page.GetByRole(AriaRole.Button, new() { Name = "Update journey" });
        private ILocator JourneyTime => _page.Locator("#option-1-heading .journey-time");
        private ILocator ErrorMessage => _page.Locator("li.field-validation-error");

        //actions

        public async Task SelectLeastWalkingRoute()
        {
            await EditPreferencesButton.ClickAsync();
            await LeastWalkingRadio.ClickAsync();
            await UpdateJourneyButton.ClickAsync();
        }



        //getters

        public async Task<bool> IsWalkingAndCyclingVisible()
        {
           await WalkingAndCyclingHeading.WaitForAsync(
                new()
                {
                    State = WaitForSelectorState.Visible,
                    Timeout = 30000
                });

            return await WalkingAndCyclingHeading.IsVisibleAsync();
        }

        public async Task<string> GetJourneyTime()
        {
            return await JourneyTime.InnerTextAsync();
        }

        public async Task<string> GetErrorMessage()
        {
            return await ErrorMessage.InnerTextAsync();
        }


    }
}
