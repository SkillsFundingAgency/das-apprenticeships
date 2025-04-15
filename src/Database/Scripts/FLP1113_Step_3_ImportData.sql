-- ========================================================================================================================
-- This script is for the purpose of migrating active non pilot apprenticeship data from commitments to apprenticeships db
-- ========================================================================================================================

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
SELECT * FROM [dbo].ApprenticeshipsToMigrate

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
    @FundingEmployerAccountIdString VARCHAR(20),
    @LegalEntityName NVARCHAR(255),
    @AccountLegalEntityId BIGINT,
    @TrainingCode NCHAR(10),
    @TrainingCourseVersion NVARCHAR(10),
    @StartDate DATETIME,
    @EndDate DATETIME,
    @TrainingPrice MONEY,
    @TrainingPriceString VARCHAR(20),
    @EndPointAssessmentPrice MONEY,
    @EndPointAssessmentPriceString VARCHAR(20),
    @TotalPrice MONEY,
    @FundingBandMaximum INT,
	@DoNotImport BIT

OPEN DataCursor
FETCH NEXT FROM DataCursor INTO
    @ApprovalsApprenticeshipId, @Uln, @FirstName, @LastName, @DateOfBirth, 
    @Ukprn, @EmployerAccountId, @FundingType, @FundingEmployerAccountIdString, @LegalEntityName,
    @AccountLegalEntityId, @TrainingCode, @TrainingCourseVersion,
    @StartDate, @EndDate, @TrainingPriceString, @EndPointAssessmentPriceString, @TotalPrice, @FundingBandMaximum, @DoNotImport

WHILE @@FETCH_STATUS = 0
BEGIN
	BEGIN IF @DoNotImport = 0
		BEGIN IF (SELECT COUNT(*) FROM [dbo].[Apprenticeship] WHERE ApprovalsApprenticeshipId = @ApprovalsApprenticeshipId) < 1

			BEGIN TRY

				SET @FundingEmployerAccountId = TRY_CAST(@FundingEmployerAccountIdString AS BIGINT)
				SET @TrainingPrice = TRY_CAST(@TrainingPriceString AS MONEY)
				SET @EndPointAssessmentPrice = TRY_CAST(@EndPointAssessmentPriceString AS MONEY)

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

		END
	END
	FETCH NEXT FROM DataCursor INTO
		@ApprovalsApprenticeshipId, @Uln, @FirstName, @LastName, @DateOfBirth, 
		@Ukprn, @EmployerAccountId, @FundingType, @FundingEmployerAccountIdString, @LegalEntityName,
		@AccountLegalEntityId, @TrainingCode, @TrainingCourseVersion,
		@StartDate, @EndDate, @TrainingPriceString, @EndPointAssessmentPriceString, @TotalPrice, @FundingBandMaximum, @DoNotImport
END

CLOSE DataCursor
DEALLOCATE DataCursor

-- =======================================================================================================================================================================================
-- STEP 5: Reporting
SELECT 'Successful Inserts' AS Type, ApprovalsApprenticeshipId FROM @SuccessIds;
IF @FailedId IS NOT NULL
    SELECT 'First Failed Insert' AS Type, @FailedId AS ApprovalsApprenticeshipId;