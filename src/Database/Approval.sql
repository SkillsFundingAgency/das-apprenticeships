CREATE TABLE [dbo].[Approval]
(
	[Id] BIGINT NOT NULL PRIMARY KEY,
	[ApprenticeshipKey] UNIQUEIDENTIFIER NOT NULL
)
GO
ALTER TABLE dbo.Approval
ADD CONSTRAINT FK_Approval_Apprenticeship FOREIGN KEY (ApprenticeshipKey) REFERENCES dbo.Apprenticeship ([Key])