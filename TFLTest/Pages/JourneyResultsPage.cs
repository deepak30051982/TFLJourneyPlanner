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
        //private ILocator FirstJourney => _page.Locator("#option-1-content > div.journey-details");
        // private ILocator FirstViewDetails => FirstJourney.GetByRole(AriaRole.Button, new() { Name = "View details" });
        private ILocator FirstViewDetails => _page.Locator("div[data-journeyindex='0'] button[data-show-text='View details']");

        //actions

        public async Task SelectLeastWalkingRouteAsync()
        {
            await EditPreferencesButton.ClickAsync();
            await LeastWalkingRadio.ClickAsync();
            await UpdateJourneyButton.ClickAsync();
        }



        //getters

        public async Task<bool> IsWalkingAndCyclingVisibleAsync()
        {
            await WalkingAndCyclingHeading.WaitForAsync(
                 new()
                 {
                     State = WaitForSelectorState.Visible,
                     Timeout = 30000
                 });

            return await WalkingAndCyclingHeading.IsVisibleAsync();
        }

        public async Task OpenJourneyDetails()
        {
            await FirstViewDetails.ClickAsync();
        }

        public async Task<string> GetJourneyTimeAsync()
        {
            return await JourneyTime.InnerTextAsync();
        }

        public async Task<string> GetErrorMessageAsync()
        {
            return await ErrorMessage.InnerTextAsync();
        }

        public async Task<IEnumerable<string>> GetAccessInformationAsync()
        {
            var heading = _page.Locator(".step-heading")
             .Filter(new LocatorFilterOptions
             { //AI help
                 Has = _page.Locator("span.location-name", new()
                 {
                     HasTextString = "Covent Garden Underground Station"
                 })
                
             }).Nth(1);

            var accessInformation = heading.Locator(".details .access-information");

            var labels = await accessInformation
                .Locator("a[aria-label]")
                .AllAsync();

            var accessInfo = new List<string>();
            foreach (var link in labels)
                accessInfo.Add(await link.GetAttributeAsync("aria-label") ?? "");

            return accessInfo;
        }


    }
}
