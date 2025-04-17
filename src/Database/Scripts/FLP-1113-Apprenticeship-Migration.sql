SET NOCOUNT ON

-- FLP-1113 Apprenticeships Migration Script
-- This script is to be run against the commitments database
-- As output, it produces another sql script that can be run against the Apprenticeships database
-- Results -> File is recommended. It may be necessary to trim execution time output from the bottom of the output file.
-- Output can be wrapped in BEGIN/COMMIT TRANSACTION statements to improve performance (recommended to do this per-chunk if splitting).

--Variables that can be modified
DECLARE @MaxApprovalDate DATETIME = '2024-04-17'; -- Date/Time max value for filtering. Used to ignore apprenticeships created after this date, due to having been captured by FLP-1110.
DECLARE @ChunkSize INT = 50000; -- This adds comment lines into the output, useful for chunking the output up into smaller bits
-- No other variables should be modified, but a TOP x can be added into the select statement for testing purposes

--Academic year boundaries
DECLARE @StartOfAcademicYear AS DATETIME = '2024-08-01';
DECLARE @EndOfAcademicYear AS DATETIME = '2025-07-31';
-- Variables for storing values
DECLARE @ApprovalsApprenticeshipId INT;
DECLARE @Uln BIGINT;
DECLARE @FirstName NVARCHAR(50);
DECLARE @LastName NVARCHAR(50);
DECLARE @DateOfBirth DATE;
DECLARE @Ukprn INT;
DECLARE @EmployerAccountId INT;
DECLARE @FundingType NVARCHAR(20);
DECLARE @FundingEmployerAccountId INT;
DECLARE @LegalEntityName NVARCHAR(255);
DECLARE @AccountLegalEntityId INT;
DECLARE @TrainingCode NVARCHAR(50);
DECLARE @TrainingCourseVersion NVARCHAR(50);
DECLARE @StartDate DATE;
DECLARE @EndDate DATE;
DECLARE @TrainingPrice DECIMAL(18, 2);
DECLARE @EndPointAssessmentPrice DECIMAL(18, 2);
DECLARE @TotalPrice DECIMAL(18, 2);
--Other variables
DECLARE @Output TABLE (OutputLine nvarchar(max), [Order] INT)
DECLARE @Count INT = 0;
DECLARE @ChunkCount INT = 0;

-- Cursor declaration
DECLARE ApprenticeshipCursor CURSOR FAST_FORWARD FOR
SELECT 
--TOP 1000 -- A top x clause can be added for testing with smaller sets of data
    Apps.Id,
    Apps.ULN,
    Apps.FirstName,
    Apps.LastName,
    Apps.DateOfBirth,
    Com.ProviderId,
    Com.EmployerAccountId,
    CASE
        WHEN Com.TransferSenderId IS NOT NULL THEN 'Transfer'
        WHEN Com.ApprenticeshipEmployerTypeOnApproval = 0 THEN 'NonLevy'
        ELSE 'Levy'
    END AS FundingType,
    Com.TransferSenderId,
    ALE.Name,
    Com.AccountLegalEntityId,
    Apps.TrainingCode,
    Apps.TrainingCourseVersion,
    Apps.StartDate,
    Apps.EndDate,
    Apps.TrainingPrice,
    Apps.EndPointAssessmentPrice,
    Apps.Cost
FROM Apprenticeship AS Apps
LEFT JOIN Commitment AS Com ON Apps.CommitmentId = Com.Id
LEFT JOIN AccountLegalEntities AS ALE ON Com.AccountLegalEntityId = ALE.Id
WHERE
	Apps.IsApproved = 1 AND
    Apps.EndDate > @StartOfAcademicYear AND
    Apps.StartDate < @EndOfAcademicYear AND
    Apps.IsOnFlexiPaymentPilot = 0 AND -- This won't actually do anything now
    (Apps.StopDate IS NULL OR Apps.StopDate > @StartOfAcademicYear) AND
	Com.EmployerAndProviderApprovedOn < @MaxApprovalDate AND
	Apps.Id NOT IN -- Ignore approvals from the pilot
	(
		'2672229',
		'2798171',
		'2748667',
		'2740299',
		'2746477',
		'2757472',
		'2778282',
		'2740310',
		'2762681',
		'2740237',
		'2798419',
		'2798160',
		'2719161',
		'2781455',
		'2744099',
		'2772594',
		'2725277',
		'2776467',
		'2787474',
		'2776925',
		'2781337',
		'2791782',
		'2769338',
		'2791621',
		'2740179',
		'2759416',
		'2724260',
		'2725310',
		'2805111',
		'2708305',
		'2760888',
		'2762817',
		'2705013',
		'2769538',
		'2759044',
		'2760087',
		'2725290',
		'2772588',
		'2786771',
		'2749757',
		'2791618',
		'2768116',
		'2783301',
		'2805028',
		'2769547',
		'2783231',
		'2805088',
		'2749775',
		'2793807',
		'2749082',
		'2749983',
		'2810748',
		'2778419',
		'2705007',
		'2718623',
		'2749522',
		'2791624',
		'2740379',
		'2783639',
		'2740318',
		'2778811',
		'2749765',
		'2772580',
		'2776539',
		'2762714',
		'2776376',
		'2786775',
		'2725324',
		'2810767',
		'2740354',
		'2723781',
		'2787513',
		'2771014',
		'2799124',
		'2748877',
		'2719341',
		'2791614',
		'2760095',
		'2749087',
		'2728733',
		'2781414',
		'2780475',
		'2722039',
		'2760881',
		'2714809'
	)

-- Open the cursor
OPEN ApprenticeshipCursor;

-- Fetch the first record
FETCH NEXT FROM ApprenticeshipCursor INTO 
    @ApprovalsApprenticeshipId,
    @Uln,
    @FirstName,
    @LastName,
    @DateOfBirth,
    @Ukprn,
    @EmployerAccountId,
    @FundingType,
    @FundingEmployerAccountId,
    @LegalEntityName,
    @AccountLegalEntityId,
    @TrainingCode,
    @TrainingCourseVersion,
    @StartDate,
    @EndDate,
    @TrainingPrice,
    @EndPointAssessmentPrice,
    @TotalPrice;

--MARK THE FIRST CHUNK
insert into @Output(OutputLine, [Order]) values ('--- CHUNK ---', 0)

-- Loop through each record
WHILE @@FETCH_STATUS = 0
BEGIN

	SET @Count = @Count + 10;
	SET @ChunkCount = @ChunkCount + 1;

	IF(@ChunkCount > @ChunkSize)
	BEGIN

		--MARK THE NEXT CHUNK
		insert into @Output(OutputLine, [Order]) values ('--- CHUNK ---', @Count-1)
		SET @ChunkCount = 0;

	END

	DECLARE @ApprenticeshipKey UNIQUEIDENTIFIER = NEWID();
	DECLARE @EpisodeKey UNIQUEIDENTIFIER = NEWID();
	DECLARE @EpisodePriceKey UNIQUEIDENTIFIER = NEWID();

	insert into @Output(OutputLine, [Order]) values
	('INSERT INTO [dbo].[Apprenticeship] ([Key], [ApprovalsApprenticeshipId], [Uln], [FirstName], [LastName], [DateOfBirth], [ApprenticeshipHashedId]) VALUES
	(''' + CONVERT(VARCHAR(36), @ApprenticeshipKey) + ''', ' + CONVERT(VARCHAR,@ApprovalsApprenticeshipId) + ', ' + CONVERT(VARCHAR,@Uln) + ', ''' + CONVERT(VARCHAR,@FirstName) + ''', ''' + CONVERT(VARCHAR,@LastName) + ''', ''' + CONVERT(VARCHAR,@DateOfBirth) + ''','''')'
	, @Count)

	insert into @Output(OutputLine, [Order]) values
	( 'INSERT INTO [dbo].[Episode] ([Key], ApprenticeshipKey, Ukprn, EmployerAccountId, FundingType, FundingPlatform, FundingEmployerAccountId, LegalEntityName, AccountLegalEntityId, TrainingCode, TrainingCourseVersion, PaymentsFrozen, LearningStatus) VALUES 
	(''' + CONVERT(VARCHAR(36),@EpisodeKey) + ''','
	+ '''' + CONVERT(VARCHAR(36),@ApprenticeshipKey) + ''','
	+ '''' + CONVERT(VARCHAR(8),@Ukprn) + ''','
	+ CONVERT(VARCHAR,@EmployerAccountId) + ','
	+ '''' + @FundingType + ''','
	+ '2,'
	+ CASE WHEN @FundingEmployerAccountId IS NULL THEN 'NULL' ELSE CONVERT(VARCHAR,@FundingEmployerAccountId) END + ','
	+ '''' + @LegalEntityName + ''','
	+ CONVERT(VARCHAR(8),@AccountLegalEntityId) + ','
	+ '''' + @TrainingCode + ''','
	+ CASE WHEN @TrainingCourseVersion IS NULL THEN 'NULL' ELSE CONVERT(VARCHAR,@TrainingCourseVersion) END + ','
	+ '0,'
	+ '''Active'')', @Count + 1)

	insert into @Output(OutputLine, [Order]) values
	('INSERT INTO [dbo].[EpisodePrice] ([Key], [EpisodeKey], [StartDate], [EndDate], [TrainingPrice], [EndPointAssessmentPrice], [TotalPrice], [FundingBandMaximum]) VALUES
		(''' + CONVERT(VARCHAR(36),@EpisodePriceKey) + ''','
	+ '''' + CONVERT(VARCHAR(36),@EpisodeKey) + ''','
	+ '''' + CONVERT(VARCHAR(10),@StartDate) + ''','
	+ '''' + CONVERT(VARCHAR(10),@EndDate) + ''','
	+ CASE WHEN @TrainingPrice IS NULL THEN 'NULL' ELSE CONVERT(VARCHAR,@TrainingPrice) END + ','
	+ CASE WHEN @EndPointAssessmentPrice IS NULL THEN 'NULL' ELSE CONVERT(VARCHAR,@EndPointAssessmentPrice) END + ','
	+ CASE WHEN @TotalPrice IS NULL THEN 'NULL' ELSE CONVERT(VARCHAR,@TotalPrice) END + ','
	+ '-1)', @Count + 2)
	
    -- Fetch the next record
    FETCH NEXT FROM ApprenticeshipCursor INTO 
        @ApprovalsApprenticeshipId,
        @Uln,
        @FirstName,
        @LastName,
        @DateOfBirth,
        @Ukprn,
        @EmployerAccountId,
        @FundingType,
        @FundingEmployerAccountId,
        @LegalEntityName,
        @AccountLegalEntityId,
        @TrainingCode,
        @TrainingCourseVersion,
        @StartDate,
        @EndDate,
        @TrainingPrice,
        @EndPointAssessmentPrice,
        @TotalPrice;
END

-- Close and deallocate the cursor
CLOSE ApprenticeshipCursor;
DEALLOCATE ApprenticeshipCursor;



SELECT OutputLine from @Output
ORDER BY [Order] ASC
