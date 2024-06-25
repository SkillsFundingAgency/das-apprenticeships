CREATE TABLE [dbo].[Episode]
(
    [Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL, 
	[IsDeleted] BIT NOT NULL DEFAULT(0), -- NOT NEEDED (for now)?
    [ApprovalsApprenticeshipId] BIGINT NOT NULL, --INFORM MIKE THAT THIS NEEDING TO BE IN DESIGN
    [Ukprn] BIGINT NOT NULL, 
    [EmployerAccountId] BIGINT NOT NULL, 
    [FundingType] NVARCHAR(50) NOT NULL, 
    [FundingPlatform] INT NULL,
    [FundingEmployerAccountId] BIGINT NULL, 
    [LegalEntityName] NVARCHAR(255) NOT NULL,
    [AccountLegalEntityId] BIGINT NULL, --INFORM MIKE THAT THIS NEEDING TO BE IN DESIGN
    [TrainingCode] NCHAR(10) NOT NULL,
    [TrainingCourseVersion] NVARCHAR(10) NULL,  --INFORM MIKE THAT THIS NEEDING TO BE IN DESIGN
    [PaymentsFrozen] BIT NOT NULL DEFAULT (0)  --INFORM MIKE THAT THIS NEEDING TO BE IN DESIGN
)
GO
ALTER TABLE dbo.Episode
ADD CONSTRAINT FK_Episode_Apprenticeship FOREIGN KEY (ApprenticeshipKey) REFERENCES dbo.Apprenticeship ([Key])
GO
CREATE INDEX IX_ApprenticeshipKey ON [dbo].[Episode] (ApprenticeshipKey);
GO