﻿CREATE TABLE [dbo].[StartDateChange]
(
	[Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL,
	[LearningKey] UNIQUEIDENTIFIER NULL,
	[ActualStartDate] DATETIME NOT NULL,
	[PlannedEndDate] DATETIME NULL,
	[Reason] NVARCHAR(MAX),
	[ProviderApprovedBy] NVARCHAR(500) NULL,
	[ProviderApprovedDate] DATETIME NULL,
	[EmployerApprovedBy] NVARCHAR(500) NULL,
	[EmployerApprovedDate] DATETIME NULL,
	[CreatedDate] DATETIME NOT NULL,
	[RequestStatus] NVARCHAR(50) NULL, 
    [Initiator] CHAR(8) NULL, 
    [RejectReason] NVARCHAR(MAX) NULL
)
GO
ALTER TABLE [dbo].[StartDateChange]
ADD CONSTRAINT FK_StartDateChange_Apprenticeship FOREIGN KEY (ApprenticeshipKey) REFERENCES dbo.Apprenticeship ([Key])
GO
CREATE INDEX IX_ApprenticeshipKey ON [dbo].[StartDateChange] (ApprenticeshipKey);
GO