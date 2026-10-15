TFL Journey Planner - Test Automation
===========================================

Overview
--------
- This repository contains 5 of the test scenarios for the TfL Journey Planner website as mentioned in the assessment. 
- The test code is implemented in C# targeting .NET 10 and uses Microsoft Playwright. 
- Tests are created with Reqnroll feature files and MSTest assertions.

Key projects
- TFL.JourneyPlanner.Core - shared playwright helpers and configuration (Playwright session, config loader).
- TFL.JourneyPlanner.Tests - test suite: Reqnroll feature file, step definitions, page objects and test hooks.

Important development decisions
--------------------------------
- Playwright for browser automation
- Static PlaywrightSession lifecycle: a singleton wrapper (StartAsync / StopAsync) used around each test run. This simplifies sharing the Playwright and Browser instances across various scenarios.
- Per-scenario browser contexts: each scenario creates its own IBrowserContext and IPage in TestHooks to isolate test state and avoid cross scenario interference. Playwright and browser triggered once reducing memory and initialization overhead
- Configuration via playwrightconfig.json: Not used, but for future extensibility.
- Page Object Model: JourneyPlannerPage and JourneyResultsPage encapsulate selectors and flows. 
- I tried mostly use ARIA roles and semantic selectors to reduce the brittleness against any future UI changes
- Explicit waits: Where appropriate, WaitForAsync is used before interacting with elements to avoid flakiness
- Non-headless by default: Browser is launched headful (Headless = false) and starts maximized
- Also added test report using reqnroll reporter to generate HTML report. go to the bin/Debug/net10.0/testresults folder and open the html file to view the report.

How to run
----------
1. Ensure .NET 10 SDK is installed.
2. dotnet restore
3. dotnet build
4. go to the Tests project folder, go to the bin/Debug/net10.0 folder and run the following command to install Playwright browsers:
pwsh bin/Debug/net10.0/playwright.ps1 install
5. dotnet test

Additional improvements
-----------------------
- Read from configurations
- Bring in parallel execution
- Add traces and screenshots and attach to report for failed scenarios
- Capture Playwright traces 
- run in multiple browsers
- Additional test scenarios



Additional Functional and Non-functional Scenarios
-----------------------

Functional scenarios
-----------------------
- Change a previously selected location and validate the results
- Enter same station in both from and to fields and validate
- Plan a journey for a future date
- Plan a journey for a past date
- Change preferences and validate the results
- validate the cockie consent behaviour for each of the preference selection
- Below from my expereince
    
- - When I clicked the "Update Preferences" button without making any changes, an error message was displayed stating: "Journey planner could not find any results for your search. Please try again."
  - I opened the Journey Planner in two separate browser windows and searched for the same journey. After clicking the button in both windows at nearly the same time, I noticed inconsistent results.
  - - In one window, only the Walking and Cycling options were displayed, along with the message:  "If you can, walk or cycle this route."
    - In the other window, three journey options were displayed,  Tube routes for the same search criteria.

Non-Functional scenarios
-----------------------
- Accessibility Testing: Validate accessibility compliance across the entire journey planning flow.( For example, using light house tool i could see some accessibility issues)
- Performance Tests: Performance under normal and peak demand (After selecting the source and destination the results was loaded very slowly)
- Additionally, lighthouse showed some performance issues due to unused files, large js files etc
- Usability Testing: Evaluate the user experience of the Journey Planner widget. For example, after clicking the "Plan Journey" button, the preferences section is not fully visible to the user
- Cross-Device and Responsive Testing: Validate the functionality and responsiveness of the journey planner across different devices and screen sizes, including desktop, tablet, and mobile platforms
- Security Testing : for common vulnerabilities

Disclaimer
-----------------------
-  90% of the development I didn't use AI as a assistance, but in some  I have used it
    * finding Accessibility options for covent garden underground station 
    ```  
    Has = _page.Locator("span.location-name", new()
           {
                     HasTextString = "Covent Garden Underground Station"
    })
    ```
    * Accept cookies overlay was not disappearing after accepting, this is an odd behaviour I found, copilot suggested below code to remove overlay
    ``` // Force-remove the lingering overlay div that blocks clicks
                //AI generated
                await _page.EvaluateAsync(@"() => {
                document.getElementById('cb-cookieoverlay')?.remove();
                document.getElementById('CybotCookiebotDialog')?.remove();
     ``` 
 - I have used visual studio 2026 for coding and it has in-built AI assistance so when I type the code, it was suggesting some of the code snippets
 - Some areas I have used google to find some approaches or fixing issues
