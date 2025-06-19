/* Pre-deployment script */
--Abort if Apprenticeship table does not exist (eg. when running in context of acceptance test)
IF EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'Apprenticeship' AND TABLE_SCHEMA = 'dbo')
BEGIN
        PRINT 'ERROR: Apprenticeship table does not exist; aborting.';
        RETURN;
END

--Abort if migration already performed
IF EXISTS (
    SELECT 1
    FROM INFORMATION_SCHEMA.TABLES
    WHERE TABLE_NAME = 'Learning' AND TABLE_SCHEMA = 'dbo'
)
BEGIN
    IF EXISTS (SELECT 1 FROM [dbo].[Learning])
    BEGIN
        PRINT 'ERROR: Learning table already populated; aborting.';
        RETURN;
    END
END
GO

/* Learning (copy of Apprenticeship) */

EXEC('
    INSERT INTO [dbo].[Learning] ([Key], [ApprovalsApprenticeshipId], [Uln], [FirstName], [LastName], [DateOfBirth], [ApprenticeshipHashedId])
    SELECT [Key], [ApprovalsApprenticeshipId], [Uln], [FirstName], [LastName], [DateOfBirth], [ApprenticeshipHashedId]
    FROM [dbo].[Apprenticeship];');

EXEC('UPDATE [Episode] set [LearningKey] = [ApprenticeshipKey];');
EXEC('UPDATE [FreezeRequest] set [LearningKey] = [ApprenticeshipKey];');
EXEC('UPDATE [PriceHistory] set [LearningKey] = [ApprenticeshipKey];');
EXEC('UPDATE [StartDateChange] set [LearningKey] = [ApprenticeshipKey];');
EXEC('UPDATE [WithdrawalRequest] set [LearningKey] = [ApprenticeshipKey];');
