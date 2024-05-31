CREATE TABLE [dbo].[FreezeRequest]
(
    [Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL, 
    [FrozenBy] NVARCHAR(500) NOT NULL, 
    [FrozenDateTime] DATETIME NOT NULL, 
    [Unfrozen] BIT NOT NULL, 
    [UnfrozenDateTime] DATETIME NULL, 
    [UnfrozenBy] NVARCHAR(500) NULL 
)

GO
ALTER TABLE [dbo].[FreezeRequest]
ADD CONSTRAINT FK_FreezeRequest_Apprenticeship FOREIGN KEY (ApprenticeshipKey) REFERENCES dbo.Apprenticeship ([Key])
GO
CREATE INDEX IX_ApprenticeshipKey ON [dbo].[FreezeRequest] (ApprenticeshipKey);
GO