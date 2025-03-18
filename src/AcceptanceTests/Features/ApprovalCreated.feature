﻿Feature: Approval Created

Scenario: Create Apprenticeship from approved commitment
	Given An apprenticeship has been created as part of the approvals journey
	Then an Apprenticeship record is created
	And an ApprenticeshipCreatedEvent event is published

Scenario: Create Apprenticeship from approved commitment with funding band maximum
	Given the funding band maximum for that apprenticeship is set
	And An apprenticeship has been created as part of the approvals journey
	Then an Apprenticeship record is created with the correct funding band maximum
	And an ApprenticeshipCreatedEvent event is published with the correct funding band maximum

Scenario: Create Apprenticeship from approved commitment with funding band maximum outside date range
	Given a funding band maximum for that apprenticeship and date range is not available
	And An apprenticeship has been created as part of the approvals journey
	Then an Apprenticeship record is not created
	And an ApprenticeshipCreatedEvent event is not published