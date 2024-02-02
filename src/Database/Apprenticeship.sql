CREATE TABLE [dbo].[Apprenticeship]
(
	[Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Uln] NVARCHAR(10) NOT NULL, 
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [DateOfBirth] DATETIME NOT NULL,
    [TrainingCode] NCHAR(10) NOT NULL,
    [ApprenticeshipHashedId] NVARCHAR(100) NULL, 
    [TrainingPrice] MONEY NULL, 
    [EndPointAssessmentPrice] MONEY NULL, 
    [TotalPrice] MONEY NULL, 
    [FundingBandMaximum] INT NULL,
    [ActualStartDate] DATETIME NULL,
    [PlannedEndDate] DATETIME NULL, 
    [AccountLegalEntityId] BIGINT NULL,
    [UKPRN] BIGINT NOT NULL DEFAULT(0),
    [EmployerAccountId] BIGINT NOT NULL DEFAULT(0)
)
GO

CREATE INDEX IX_Ukprn ON [dbo].[Apprenticeship] (UKPRN);
GO

CREATE INDEX IX_EmployerAccountId ON [dbo].[Apprenticeship] (EmployerAccountId);
GO