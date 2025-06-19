/* Pre-deployment script */

EXEC('
    INSERT INTO [dbo].[Learning] ([Key], [ApprovalsApprenticeshipId], [Uln], [FirstName], [LastName], [DateOfBirth], [ApprenticeshipHashedId])
    SELECT A.[Key], A.[ApprovalsApprenticeshipId], A.[Uln], A.[FirstName], A.[LastName], A.[DateOfBirth], A.[ApprenticeshipHashedId]
    FROM [dbo].[Apprenticeship] A
    WHERE NOT EXISTS (
        SELECT 1
        FROM [dbo].[Learning] L
        WHERE L.[Key] = A.[Key]
    );');

EXEC('UPDATE [Episode] set [LearningKey] = [ApprenticeshipKey];');
EXEC('UPDATE [FreezeRequest] set [LearningKey] = [ApprenticeshipKey];');
EXEC('UPDATE [PriceHistory] set [LearningKey] = [ApprenticeshipKey];');
EXEC('UPDATE [StartDateChange] set [LearningKey] = [ApprenticeshipKey];');
EXEC('UPDATE [WithdrawalRequest] set [LearningKey] = [ApprenticeshipKey];');
