CREATE TABLE [dbo].[StartDateChange]
(
	[Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[LearningKey] UNIQUEIDENTIFIER NOT NULL,
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
ADD CONSTRAINT FK_StartDateChange_Learning FOREIGN KEY (LearningKey) REFERENCES dbo.Learning ([Key])
GO
CREATE INDEX IX_LearningKey ON [dbo].[StartDateChange] (LearningKey);
GO