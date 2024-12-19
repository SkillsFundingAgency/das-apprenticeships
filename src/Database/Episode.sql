CREATE TABLE [dbo].[Episode]
(
    [Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL, 
	[IsDeleted] BIT NOT NULL DEFAULT(0),
    [Ukprn] BIGINT NOT NULL, 
    [EmployerAccountId] BIGINT NOT NULL, 
    [FundingType] NVARCHAR(50) NOT NULL, 
    [FundingPlatform] INT NULL,
    [FundingEmployerAccountId] BIGINT NULL, 
    [LegalEntityName] NVARCHAR(255) NOT NULL,
    [AccountLegalEntityId] BIGINT NULL,
    [TrainingCode] NCHAR(10) NOT NULL,
    [TrainingCourseVersion] NVARCHAR(10) NULL,
    [PaymentsFrozen] BIT NOT NULL DEFAULT (0), 
    [LearningStatus] NVARCHAR(50) NOT NULL DEFAULT 'Active', 
    [LastDayOfLearning] DATETIME NULL
)
GO
ALTER TABLE dbo.Episode
ADD CONSTRAINT FK_Episode_Apprenticeship FOREIGN KEY (ApprenticeshipKey) REFERENCES dbo.Apprenticeship ([Key])
GO
CREATE INDEX IX_ApprenticeshipKey ON [dbo].[Episode] (ApprenticeshipKey);
GO