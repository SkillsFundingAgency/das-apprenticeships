CREATE TABLE [dbo].[Approval]
(
	[Id] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL, 
    [ApprovalsApprenticeshipId] BIGINT NOT NULL, 
    [LegalEntityName] NVARCHAR(255) NOT NULL, 
    [ActualStartDate] DATETIME NULL, 
    [PlannedEndDate] DATETIME NOT NULL, 
    [AgreedPrice] MONEY NOT NULL, 
    [FundingEmployerAccountId] BIGINT NULL, 
    [FundingType] NVARCHAR(50) NOT NULL, 
    [FundingBandMaximum] INT NOT NULL, 
    [PlannedStartDate] DATETIME NULL, 
    [FundingPlatform] INT NULL
)
GO
ALTER TABLE dbo.Approval
ADD CONSTRAINT FK_Approval_Apprenticeship FOREIGN KEY (ApprenticeshipKey) REFERENCES dbo.Apprenticeship ([Key])