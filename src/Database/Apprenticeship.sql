CREATE TABLE [dbo].[Apprenticeship]
(
	[Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Uln] NVARCHAR(10) NOT NULL, 
    [DateOfBirth] DATETIME NOT NULL,
    [TrainingCode] NCHAR(10) NOT NULL
)
