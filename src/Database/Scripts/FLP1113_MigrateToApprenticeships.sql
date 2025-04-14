-- ========================================================================================================================
-- This script is for the purpose of migrating active non pilot apprenticeship data from commitments to apprenticeships db
-- ========================================================================================================================

DECLARE @StartOfAcademicYear AS DATETIME = '2024-08-01';
DECLARE @EndOfAcademicYear AS DATETIME = '2025-07-31';

-- STEP 1: Create temp table
USE [SFA.DAS.Apprenticeships.Database];

CREATE TABLE #ApprenticeshipsToMigrate
(
    ApprovalsApprenticeshipId BIGINT,
    Uln NVARCHAR(10),
    FirstName NVARCHAR(100),
    LastName NVARCHAR(100),
    DateOfBirth DATETIME,
    Ukprn BIGINT,
    EmployerAccountId BIGINT,
    FundingType NVARCHAR(50),
    FundingEmployerAccountId BIGINT,
    LegalEntityName NVARCHAR(255),
    AccountLegalEntityId BIGINT,
    TrainingCode NCHAR(10),
    TrainingCourseVersion NVARCHAR(10),
    StartDate DATETIME,
    EndDate DATETIME,
    TrainingPrice MONEY,
    EndPointAssessmentPrice MONEY,
    TotalPrice MONEY,
    FundingBandMaximum INT
);
-- =======================================================================================================================================================================================

-- STEP 2: Insert data into temp table (can be from SELECT or manual VALUES)
INSERT INTO #ApprenticeshipsToMigrate
SELECT
		Apps.Id AS ApprovalsApprenticeshipId,
		Apps.ULN AS Uln,
		Apps.FirstName AS FirstName,
		Apps.LastName AS LastName,
		Apps.DateOfBirth AS DateOfBirth,
		Com.ProviderId AS Ukprn,
		Com.EmployerAccountId AS EmployerAccountId,
		CASE
			WHEN Com.TransferSenderId IS NOT NULL THEN 'Transfer'
			WHEN Com.ApprenticeshipEmployerTypeOnApproval = 0 THEN 'NonLevy'
			ELSE 'Levy'
		END AS FundingType,
		Com.TransferSenderId AS FundingEmployerAccountId,
		ALE.Name AS LegalEntityName,
		Com.AccountLegalEntityId AS AccountLegalEntityId,
		Apps.TrainingCode AS TrainingCode,
		Apps.TrainingCourseVersion AS TrainingCourseVersion,
		Apps.StartDate AS StartDate,
		Apps.EndDate AS EndDate,
		Apps.TrainingPrice AS TrainingPrice,
		Apps.EndPointAssessmentPrice AS EndPointAssessmentPrice,
		Apps.Cost AS TotalPrice,
		-1 AS FundingBandMaximum
	FROM [SFA.DAS.Commitments.Database].[dbo].[Apprenticeship] AS Apps
	LEFT JOIN [SFA.DAS.Commitments.Database].[dbo].[Commitment] AS Com ON Apps.CommitmentId = Com.Id
	LEFT JOIN [SFA.DAS.Commitments.Database].[dbo].AccountLegalEntities AS ALE ON Com.AccountLegalEntityId = ALE.Id
	WHERE
		Apps.EndDate > @StartOfAcademicYear AND
		Apps.StartDate < @EndOfAcademicYear AND
		Apps.IsOnFlexiPaymentPilot = 0 AND
		(Apps.StopDate IS NULL OR Apps.StopDate > @StartOfAcademicYear)

-- =======================================================================================================================================================================================
-- STEP 3: Declare tracking table
DECLARE @SuccessIds TABLE (ApprovalsApprenticeshipId BIGINT);
DECLARE @FailedId BIGINT;

-- =======================================================================================================================================================================================
-- STEP 4: Iterate Temp Table inserting into apprenticeships DB
DECLARE @ApprenticeshipKey UNIQUEIDENTIFIER
DECLARE @EpisodeKey UNIQUEIDENTIFIER
DECLARE @EpisodePriceKey UNIQUEIDENTIFIER

DECLARE DataCursor CURSOR FOR
SELECT * FROM #ApprenticeshipsToMigrate

DECLARE 
    @ApprovalsApprenticeshipId BIGINT,
    @Uln NVARCHAR(10),
    @FirstName NVARCHAR(100),
    @LastName NVARCHAR(100),
    @DateOfBirth DATETIME,
    @Ukprn BIGINT,
    @EmployerAccountId BIGINT,
    @FundingType NVARCHAR(50),
    @FundingEmployerAccountId BIGINT,
    @LegalEntityName NVARCHAR(255),
    @AccountLegalEntityId BIGINT,
    @TrainingCode NCHAR(10),
    @TrainingCourseVersion NVARCHAR(10),
    @StartDate DATETIME,
    @EndDate DATETIME,
    @TrainingPrice MONEY,
    @EndPointAssessmentPrice MONEY,
    @TotalPrice MONEY,
    @FundingBandMaximum INT

OPEN DataCursor
FETCH NEXT FROM DataCursor INTO
    @ApprovalsApprenticeshipId, @Uln, @FirstName, @LastName, @DateOfBirth, 
    @Ukprn, @EmployerAccountId, @FundingType, @FundingEmployerAccountId, @LegalEntityName,
    @AccountLegalEntityId, @TrainingCode, @TrainingCourseVersion,
    @StartDate, @EndDate, @TrainingPrice, @EndPointAssessmentPrice, @TotalPrice, @FundingBandMaximum

WHILE @@FETCH_STATUS = 0
BEGIN
	BEGIN TRY
			BEGIN TRANSACTION;
				SET @ApprenticeshipKey = NEWID()
				SET @EpisodeKey = NEWID()
				SET @EpisodePriceKey = NEWID()

				INSERT INTO [dbo].[Apprenticeship]
					([Key], [ApprovalsApprenticeshipId], [Uln], [FirstName], [LastName], [DateOfBirth])
				VALUES
					(@ApprenticeshipKey, @ApprovalsApprenticeshipId, @Uln, @FirstName, @LastName, @DateOfBirth)

				INSERT INTO [dbo].[Episode]
					([Key], ApprenticeshipKey, Ukprn, EmployerAccountId, FundingType, FundingPlatform, FundingEmployerAccountId, LegalEntityName, AccountLegalEntityId, TrainingCode, TrainingCourseVersion, PaymentsFrozen, LearningStatus)
				VALUES
					(@EpisodeKey, @ApprenticeshipKey, @Ukprn, @EmployerAccountId, @FundingType, 2, @FundingEmployerAccountId, @LegalEntityName, @AccountLegalEntityId, @TrainingCode, @TrainingCourseVersion, 0, 'Active')

				INSERT INTO [dbo].[EpisodePrice]
					([Key], [EpisodeKey], [StartDate], [EndDate], [TrainingPrice], [EndPointAssessmentPrice], [TotalPrice], [FundingBandMaximum])
				VALUES
					(@EpisodePriceKey, @EpisodeKey, @StartDate, @EndDate, @TrainingPrice, @EndPointAssessmentPrice, @TotalPrice, @FundingBandMaximum)

			COMMIT TRANSACTION;

			INSERT INTO @SuccessIds (ApprovalsApprenticeshipId) VALUES (@ApprovalsApprenticeshipId);
    END TRY
    BEGIN CATCH
        ROLLBACK TRANSACTION;
        SET @FailedId = @ApprovalsApprenticeshipId;
        BREAK;
    END CATCH;

	FETCH NEXT FROM DataCursor INTO
		@ApprovalsApprenticeshipId, @Uln, @FirstName, @LastName, @DateOfBirth, 
		@Ukprn, @EmployerAccountId, @FundingType, @FundingEmployerAccountId, @LegalEntityName,
		@AccountLegalEntityId, @TrainingCode, @TrainingCourseVersion,
		@StartDate, @EndDate, @TrainingPrice, @EndPointAssessmentPrice, @TotalPrice, @FundingBandMaximum
END

CLOSE DataCursor
DEALLOCATE DataCursor

-- =======================================================================================================================================================================================
-- STEP 5: Reporting
SELECT 'Successful Inserts' AS Type, ApprovalsApprenticeshipId FROM @SuccessIds;
IF @FailedId IS NOT NULL
    SELECT 'First Failed Insert' AS Type, @FailedId AS ApprovalsApprenticeshipId;