CREATE TABLE [dbo].[PriceHistory]
(
	[ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL,
    [TrainingPrice] MONEY NOT NULL,
    [AssessmentPrice] MONEY NOT NULL,
    [TotalPrice] MONEY NOT NULL,
    [EffectiveFrom] DATETIME NOT NULL,
    [ApprovedDate] DATETIME NOT NULL
)
GO
ALTER TABLE dbo.PriceHistory
ADD CONSTRAINT FK_PriceHistory_Apprenticeship FOREIGN KEY (ApprenticeshipKey) REFERENCES dbo.Apprenticeship ([Key])

GO
CREATE NONCLUSTERED INDEX [IX_PriceHistory__ApprenticeshipKey]
    ON [dbo].[PriceHistory]([ApprenticeshipKey] ASC);