﻿CREATE TABLE [dbo].[WithdrawalRequest]
(
    [Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL, 
    [LearningKey] UNIQUEIDENTIFIER NULL,
    [EpisodeKey] UNIQUEIDENTIFIER NOT NULL, 
    [Reason] NVARCHAR(100) NOT NULL, 
    [LastDayOfLearning] DATETIME NOT NULL, 
    [CreatedDate] DATETIME NOT NULL, 
    [ProviderApprovedBy] NVARCHAR(500) NOT NULL
)
GO
ALTER TABLE dbo.WithdrawalRequest
ADD CONSTRAINT FK_WithdrawalRequest_Episode FOREIGN KEY (EpisodeKey) REFERENCES dbo.Episode ([Key])
GO
CREATE INDEX IX_ApprenticeshipKey ON [dbo].[WithdrawalRequest] (ApprenticeshipKey);
GO