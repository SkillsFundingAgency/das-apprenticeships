/*
Post-deployment script
*/

IF EXISTS (SELECT 1 FROM [Learning])
BEGIN
    PRINT 'ERROR: Learning table already populated; aborting.';
    RETURN;
END
GO

/* Learning (copy of Apprenticeship) */

EXEC('
    INSERT INTO [dbo].[Learning] ([Key], [ApprovalsApprenticeshipId], [Uln], [FirstName], [LastName], [DateOfBirth], [ApprenticeshipHashedId])
    SELECT [Key], [ApprovalsApprenticeshipId], [Uln], [FirstName], [LastName], [DateOfBirth], [ApprenticeshipHashedId]
    FROM [dbo].[Apprenticeship];');
