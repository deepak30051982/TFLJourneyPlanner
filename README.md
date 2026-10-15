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

Functional scenarios
-----------------------
- Change a previously selected location and validate the results
- Enter same station in both from and to fields and validate
- Plan a journey for a future date
- Plan a journey for a past date
- Change preferences and validate the results
- I clicked on the update preference button without any changes and i could see it is showing an error - Journey planner could not find any results to your search. Please try again
- validate the cockie consent behaviour for each of the preference selection
- I opened two windows and tried the same jorney plan, clicked the button almost at the same time
  in one window it only showed cycling and walking options with a message 'If you can, walk or cycle this route.'
  but in the other window it showed 3 options with options including tube 




Non-Functional scenarios
-----------------------
- Accessibility throughout the journey flow (using light house tool i could see some accessibility issues)
- Performance under normal and peak demand (After selecting the source and destination the results was loaded very slowly)
- Also lighthouse showed some performance issues due to unused files, large js files etc
- Usability testing for the journy planner widget (after clicking on plan jorney button the preferences are is not visible)
- DIfferent digital channel such as mobile, tablet and desktop to validate the responsiveness

Disclaimer
-----------------------
- Mostly 90% of the development i didn't used AI as a assistance, but for some of the areas I have used it
    * for accessibility options for covent garden underground station finding below code
    ```  
    Has = _page.Locator("span.location-name", new()
           {
                     HasTextString = "Covent Garden Underground Station"
    })
    ```
    * Accept cookies overlay was not disappearing even after clicking, this is an odd behaviour i found
    ``` // Force-remove the lingering overlay div that blocks clicks
                //AI generated
                await _page.EvaluateAsync(@"() => {
                document.getElementById('cb-cookieoverlay')?.remove();
                document.getElementById('CybotCookiebotDialog')?.remove();
     ``` 
 - I have used visual studio 2026 for coding and it has in-built AI assistance so when I type the code, it was suggesting some of the code snippets
 - Some areas I have used google to find some approaches or fixing issues