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

        public async Task GoToAsync(string url)
        {
            await _page.GotoAsync(url);
            await AcceptCookiesAsync();
            await _page.GotoAsync(url);
        }

        public async Task AcceptCookiesAsync()
        {
            

            //accept cookies
            
            try
            {
                await CookieAcceptAllButton.WaitForAsync(
                    new() { State = WaitForSelectorState.Visible, Timeout = 8000 });
                await CookieAcceptAllButton.ClickAsync();
                await CookieAcceptAllButton.WaitForAsync(
                    new() { State = WaitForSelectorState.Hidden, Timeout = 8000 });
            }
            catch
            {
                // Force-remove the lingering overlay div that blocks clicks
                //AI generated
                await _page.EvaluateAsync(@"() => {
                document.getElementById('cb-cookieoverlay')?.remove();
                document.getElementById('CybotCookiebotDialog')?.remove();
            }");
            }
        }

        public async Task SelectFromLocationAsync(string searchText, string stationText)
        {
            await FromLocation.WaitForAsync();
            await FromLocation.FillAsync(searchText);
            await _page.GetByText(stationText, new() { Exact = true }).ClickAsync();
        }

        public async Task SelectToLocationAsync(string searchText, string stationText)
        {
            await ToLocation.WaitForAsync();
            await ToLocation.FillAsync(searchText);
            await _page.GetByText(stationText, new() { Exact = true }).ClickAsync();
        }

        public async Task EnterInvalidLocationsAsync(string from, string to)
        {
            await FromLocation.FillAsync(from);
            await ToLocation.FillAsync(to);
        }

        public async Task ClickPlanJourneyAsync()
        {
            await PlanJourney.ClickAsync();
        }

        //getters

        public async Task<string> GetInvalidJourneyErrorAsync()
        {
            return await InvalidJourneyError.InnerTextAsync();
        }

        public async Task<string> GetFromRequiredErrorAsync()
        {
            return await FromRequiredError.InnerTextAsync();
        }

        public async Task<string> GetToRequiredErrorAsync()
        {
            return await ToRequiredError.InnerTextAsync();
        }

        public string CurrentUrl => _page.Url;
    }
}
