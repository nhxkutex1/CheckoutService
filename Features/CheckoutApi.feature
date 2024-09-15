Feature: Checkout Service API

Scenario: Group of 4 orders 4 starters, 4 mains, and 4 drinks
	Given a group of 4 people order 4 starters, 4 mains, and 4 drinks at <time>
	When the bill is requested via the API
	Then the API response status code should be 200
	And the total bill should be calculated based on current prices

Scenario: Group of 2 orders before 19:00 and is joined by 2 more people at 20:00
	Given a group of 2 people order 1 starters, 2 mains, and 2 drinks at 18:00
	When the bill is requested via the API
	Then the API response status code should be 200
	And the total bill should be £21.50
	When 2 more people join at 20:00 and order 2 mains and 2 drinks
	And the bill is requested via the API
	Then the API response status code should be 200
	And the total bill should be £40.50

Scenario: Group of 4 cancels one order
	Given a group of 4 people order 4 starters, 4 mains, and 4 drinks at <time>
	When the bill is requested via the API
	Then the API response status code should be 200
	And the total bill should be calculated based on current prices
	When a member cancels their order by amount of 1 each item
	And the bill is requested via the API
	Then the API response status code should be 200
	And the total bill should be calculated based on current prices
