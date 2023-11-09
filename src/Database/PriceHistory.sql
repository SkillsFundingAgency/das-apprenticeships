CREATE TABLE [dbo].[PriceHistory]
(
	[Key] UNIQUEIDENTIFIER PRIMARY KEY,
    [ApprenticeshipKey] UNIQUEIDENTIFIER,
    [ApprenticeshipId] BIGINT,
    [TrainingPrice] DECIMAL(18, 2),
    [AssessmentPrice] DECIMAL(18, 2),
    [TotalPrice] DECIMAL(18, 2) NOT NULL,
    [EffectiveFromDate] DATETIME,
    [ProviderApprovedBy] NVARCHAR(500),
    [ProviderApprovedDate] DATETIME,
    [EmployerApprovedBy] NVARCHAR(500),
    [EmployerApprovedDate] DATETIME,
    [CreatedDate] DATETIME,
    [PriceChangeRequestStatus] SMALLINT
)
GO
ALTER TABLE [dbo].[PriceHistory]
ADD CONSTRAINT FK_PriceHistory_Apprenticeship FOREIGN KEY (ApprenticeshipKey) REFERENCES dbo.Apprenticeship ([Key])
GO