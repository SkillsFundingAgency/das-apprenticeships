Feature: Approval Created

Scenario: Simple Earnings Generation
	Given An apprenticeship has been created as part of the approvals journey
	Then an Apprenticeship record is created
	And an ApprenticeshipCreatedEvent event is published