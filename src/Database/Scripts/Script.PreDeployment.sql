/*
Pre-deployment script
*/

IF EXISTS (SELECT 1 FROM sys.tables WHERE name = 'Learning' AND schema_id = SCHEMA_ID('dbo'))
BEGIN
    PRINT 'Learning table already exists. Migration skipped.';
    RETURN;
END
GO

/* Learning (copy of Apprenticeship) */

CREATE TABLE [dbo].[Learning]
(
	[Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ApprovalsApprenticeshipId] BIGINT NOT NULL,
    [Uln] NVARCHAR(10) NOT NULL, 
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [DateOfBirth] DATETIME NOT NULL,
    [ApprenticeshipHashedId] NVARCHAR(100) NULL
)
GO
CREATE INDEX IX_Learning_Uln ON Learning (Uln)
GO


INSERT INTO [dbo].[Learning] ([Key], [ApprovalsApprenticeshipId], [Uln], [FirstName], [LastName], [DateOfBirth], [ApprenticeshipHashedId])
SELECT 
	[Key],
	[ApprovalsApprenticeshipId], 
	[Uln], 
	[FirstName], 
	[LastName], 
	[DateOfBirth], 
	[ApprenticeshipHashedId]
FROM [dbo].[Apprenticeship];
GO

/* Episode */

DROP INDEX IX_ApprenticeshipKey ON dbo.Episode;
GO

ALTER TABLE dbo.Episode
DROP CONSTRAINT FK_Episode_Apprenticeship;
GO

ALTER TABLE dbo.Episode ADD LearningKey UNIQUEIDENTIFIER NULL;
GO

UPDATE dbo.Episode
SET LearningKey = ApprenticeshipKey;
GO

ALTER TABLE dbo.Episode DROP COLUMN ApprenticeshipKey;
GO

ALTER TABLE dbo.Episode
ADD CONSTRAINT FK_Episode_Learning FOREIGN KEY (LearningKey) REFERENCES dbo.Learning ([Key]);
GO

CREATE INDEX IX_LearningKey ON dbo.Episode (LearningKey);
GO


/* FreezeRequest */

ALTER TABLE dbo.FreezeRequest
DROP CONSTRAINT FK_FreezeRequest_Apprenticeship;
GO

ALTER TABLE dbo.FreezeRequest ADD LearningKey UNIQUEIDENTIFIER NULL;
GO

UPDATE dbo.FreezeRequest
SET LearningKey = ApprenticeshipKey;
GO

ALTER TABLE dbo.FreezeRequest DROP COLUMN ApprenticeshipKey;
GO

ALTER TABLE dbo.FreezeRequest
ADD CONSTRAINT FK_FreezeRequest_Learning FOREIGN KEY (LearningKey) REFERENCES dbo.Learning ([Key]);
GO

DROP INDEX IX_ApprenticeshipKey ON dbo.FreezeRequest;
GO

CREATE INDEX IX_LearningKey ON dbo.FreezeRequest (LearningKey);
GO

/* Price History */

ALTER TABLE dbo.PriceHistory ADD LearningKey UNIQUEIDENTIFIER NULL;
GO

UPDATE dbo.PriceHistory
SET LearningKey = ApprenticeshipKey;
GO

ALTER TABLE dbo.PriceHistory DROP COLUMN ApprenticeshipKey;
GO

ALTER TABLE dbo.PriceHistory
DROP CONSTRAINT FK_PriceHistory_Apprenticeship;
GO

ALTER TABLE dbo.PriceHistory
ADD CONSTRAINT FK_PriceHistory_Learning FOREIGN KEY (LearningKey) REFERENCES dbo.Learning ([Key]);
GO

DROP INDEX IX_ApprenticeshipKey ON dbo.PriceHistory;
GO

CREATE INDEX IX_LearningKey ON dbo.PriceHistory (LearningKey);
GO