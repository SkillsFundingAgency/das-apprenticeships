CREATE TABLE [dbo].[EpisodePrice]
(
    [Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
	[EpisodeKey] UNIQUEIDENTIFIER NOT NULL, 
    [IsDeleted] BIT NOT NULL DEFAULT(0),
    [StartDate] DATETIME NULL, 
    [EndDate] DATETIME NOT NULL, 
    [TrainingPrice] MONEY NULL, 
    [EndPointAssessmentPrice] MONEY NULL, 
    [TotalPrice] MONEY NULL, 
    [FundingBandMaximum] INT NOT NULL
)
GO
ALTER TABLE dbo.EpisodePrice
ADD CONSTRAINT FK_EpisodePrice_Episode FOREIGN KEY (EpisodeKey) REFERENCES dbo.Episode ([Key])
GO
CREATE INDEX IX_EpisodeKey ON [dbo].[EpisodePrice] (EpisodeKey);
GO