using Microsoft.Playwright;

namespace TFL.JourneyPlanner.Tests.Pages
{
    public class JourneyPlannerPage
    {
        private readonly IPage _page;

        public JourneyPlannerPage(IPage page)
        {
            _page = page;
        }

        //locators

        private ILocator FromLocation => _page.GetByRole(AriaRole.Combobox, new() { Name = "From location" });
        private ILocator ToLocation => _page.GetByRole(AriaRole.Combobox, new() { Name = "To location" });
        private ILocator PlanJourney => _page.GetByRole(AriaRole.Button, new() { Name = "Plan my journey" });
        private ILocator InvalidJourneyError => _page.Locator("li.field-validation-error");
        private ILocator FromRequiredError => _page.Locator("#InputFrom-error");
        private ILocator ToRequiredError => _page.Locator("#InputTo-error");

        private ILocator CookieAcceptAllButton => _page.GetByRole(AriaRole.Button, new() { Name = "Accept all cookies" });

        //actions

        public async Task Goto(string url)
        {
            await _page.GotoAsync(url);
            await AcceptCookies();
            await _page.GotoAsync(url);
        }

        public async Task AcceptCookies()
        {
            

            //accept cookies
            
            try
            {
                await CookieAcceptAllButton.WaitForAsync(
                    new() { State = WaitForSelectorState.Visible, Timeout = 8_000 });
                await CookieAcceptAllButton.ClickAsync();
                await CookieAcceptAllButton.WaitForAsync(
                    new() { State = WaitForSelectorState.Hidden, Timeout = 8_000 });
            }
            catch
            {
                // Banner not present — safe to continue
            }
        }

        public async Task SelectFromLocation(string searchText, string stationText)
        {
            await FromLocation.WaitForAsync();
            await FromLocation.FillAsync(searchText);
            await _page.GetByText(stationText, new() { Exact = true }).ClickAsync();
        }

        public async Task SelectToLocation(string searchText, string stationText)
        {
            await ToLocation.WaitForAsync();
            await ToLocation.FillAsync(searchText);
            await _page.GetByText(stationText, new() { Exact = true }).ClickAsync();
        }

        public async Task EnterInvalidLocations(string from, string to)
        {
            await FromLocation.FillAsync(from);
            await ToLocation.FillAsync(to);
        }

        public async Task ClickPlanJourney()
        {
            await PlanJourney.ClickAsync();
        }

        //getters

        public async Task<string> GetInvalidJourneyError()
        {
            return await InvalidJourneyError.InnerTextAsync();
        }

        public async Task<string> GetFromRequiredError()
        {
            return await FromRequiredError.InnerTextAsync();
        }

        public async Task<string> GetToRequiredError()
        {
            return await ToRequiredError.InnerTextAsync();
        }

        public string CurrentUrl => _page.Url;
    }
}
