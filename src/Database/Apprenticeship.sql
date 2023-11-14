CREATE TABLE [dbo].[Apprenticeship]
(
	[Key] UNIQUEIDENTIFIER NOT NULL PRIMARY KEY, 
    [Uln] NVARCHAR(10) NOT NULL, 
    [FirstName] NVARCHAR(100) NOT NULL,
    [LastName] NVARCHAR(100) NOT NULL,
    [DateOfBirth] DATETIME NOT NULL,
    [TrainingCode] NCHAR(10) NOT NULL,
    [ApprenticeshipHashedId] NVARCHAR(100) NULL, 
    [TrainingPrice] MONEY NULL, 
    [EndPointAssessmentPrice] MONEY NULL, 
    [TotalPrice] MONEY NULL
)
