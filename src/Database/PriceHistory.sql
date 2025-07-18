﻿CREATE TABLE [dbo].[PriceHistory]
(
	[Key] UNIQUEIDENTIFIER PRIMARY KEY,
    [LearningKey] UNIQUEIDENTIFIER NOT NULL,
    [TrainingPrice] DECIMAL(18, 2) NULL,
    [AssessmentPrice] DECIMAL(18, 2) NULL,
    [TotalPrice] DECIMAL(18, 2) NOT NULL,
    [EffectiveFromDate] DATETIME NOT NULL,
    [ProviderApprovedBy] NVARCHAR(500) NULL,
    [ProviderApprovedDate] DATETIME NULL,
    [EmployerApprovedBy] NVARCHAR(500) NULL,
    [EmployerApprovedDate] DATETIME NULL,
    [ChangeReason] NVARCHAR(MAX) NULL,
    [CreatedDate] DATETIME NOT NULL,
    [PriceChangeRequestStatus] NVARCHAR(50) NULL,
    [RejectReason] NVARCHAR(MAX) NULL, 
    [Initiator] CHAR(8) NULL
)
GO
ALTER TABLE [dbo].[PriceHistory]
ADD CONSTRAINT FK_PriceHistory_Learning FOREIGN KEY (LearningKey) REFERENCES dbo.Learning ([Key])
GO
CREATE INDEX IX_LearningKey ON [dbo].[PriceHistory] (LearningKey);
GO