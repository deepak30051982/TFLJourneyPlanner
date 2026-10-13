Feature: TfL journey planner
  As a commut
  I want to plan journeys using TfL's journey planner
  So that I can compare travel options and station accessibility information

Background:
	Given I am on the TfL Plan a journey page

@happypath
Scenario: Compare walking and cycling options between nearby stations
	When I plan a journey from "Leicester Square Underground Station" to "Covent Garden Underground Station"
	Then walking and cycling options should be displayed

@happypath
Scenario: Update journey preferences to least walking
	When I plan a journey from "Leicester Square Underground Station" to "Covent Garden Underground Station"
	And I choose routes with least walking
	Then the journey results should show a journey time

@happypath
Scenario: View complete access information for Covent Garden station
	When I plan a journey from "Leicester Square Underground Station" to "Covent Garden Underground Station"
	And I open journey details
	Then complete access information should be available

@happypath
Scenario: Invalid locations do not produce journey results
	When I enter invalid journey locations
	Then the planner should not show any journey results

@happypath
Scenario: Empty locations should show an error message
	When I submit the journey planner with no locations
	Then journey planning should not proceed
	And an error message should be displayed
