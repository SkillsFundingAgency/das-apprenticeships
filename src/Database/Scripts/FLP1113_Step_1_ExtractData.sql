-- ======================================================================================================
-- This Extracts data from commitments db
-- Assumes this will be run from sql server management studio
-- Before Running, make sure headers will be included in results
--	-Navigate to Query
--	-Select Query Options
--	-Select Results, then Grid
--	-Ensure "Include column headers when copying or saving the results" is checked
-- save to csv named ApprenticeshipsToMigrate.csv
-- ======================================================================================================

DECLARE @StartOfAcademicYear AS DATETIME = '2024-08-01';
DECLARE @EndOfAcademicYear AS DATETIME = '2025-07-31';

-- Dummy boundary rows first
SELECT 
    CAST(1 AS BIGINT) AS ApprovalsApprenticeshipId,
    N'1234567890' AS Uln,
    N'John' AS FirstName,
    N'Smith' AS LastName,
    CAST('1990-01-01' AS DATETIME) AS DateOfBirth,
    CAST(10000001 AS BIGINT) AS Ukprn,
    CAST(20000001 AS BIGINT) AS EmployerAccountId,
    N'Levy' AS FundingType,
    CAST(20000002 AS BIGINT) AS FundingEmployerAccountId,
    N'Example Legal Entity Ltd' AS LegalEntityName,
    CAST(30000001 AS BIGINT) AS AccountLegalEntityId,
    N'STD001    ' AS TrainingCode, -- padded to 10 chars
    N'v1' AS TrainingCourseVersion,
    CAST('2024-09-01' AS DATETIME) AS StartDate,
    CAST('2025-08-31' AS DATETIME) AS EndDate,
    CAST(5000.00 AS MONEY) AS TrainingPrice,
    CAST(1000.00 AS MONEY) AS EndPointAssessmentPrice,
    CAST(6000.00 AS MONEY) AS TotalPrice,
    CAST(7000 AS INT) AS FundingBandMaximum,
    CAST(1 AS BIT) AS DoNotImport

UNION ALL

SELECT 
    CAST(9223372036854775807 AS BIGINT),
    N'9999999999',
    N'AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA',
    N'BBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBBB',
    CAST('9999-12-31' AS DATETIME),
    CAST(999999999999999 AS BIGINT),
    CAST(999999999999999 AS BIGINT),
    N'Transfer',
    CAST(999999999999999 AS BIGINT),
    REPLICATE(N'L', 255),
    CAST(999999999999999 AS BIGINT),
    N'STD999    ',
    N'Version999',
    CAST('1900-01-01' AS DATETIME),
    CAST('9999-12-31' AS DATETIME),
    CAST(9999999.99 AS MONEY),
    CAST(9999999.99 AS MONEY),
    CAST(19999999.98 AS MONEY),
    CAST(2147483647 AS INT),
    CAST(1 AS BIT)

UNION ALL

SELECT 
    CAST(0 AS BIGINT),
    N'0000000000',
    N'Z',
    N'Y',
    CAST('2000-01-01' AS DATETIME),
    CAST(0 AS BIGINT),
    CAST(0 AS BIGINT),
    N'NonLevy',
    CAST(0 AS BIGINT),
    N'Z Org',
    CAST(0 AS BIGINT),
    N'TRN000    ',
    N'v0',
    CAST('2023-01-01' AS DATETIME),
    CAST('2023-12-31' AS DATETIME),
    CAST(0.00 AS MONEY),
    CAST(0.00 AS MONEY),
    CAST(0.00 AS MONEY),
    CAST(0 AS INT),
    CAST(1 AS BIT)

-- Now real data
UNION ALL

SELECT 
    Apps.Id AS ApprovalsApprenticeshipId,
    Apps.ULN AS Uln,
    Apps.FirstName,
    Apps.LastName,
    Apps.DateOfBirth,
    Com.ProviderId AS Ukprn,
    Com.EmployerAccountId,
    CASE
        WHEN Com.TransferSenderId IS NOT NULL THEN 'Transfer'
        WHEN Com.ApprenticeshipEmployerTypeOnApproval = 0 THEN 'NonLevy'
        ELSE 'Levy'
    END AS FundingType,
    Com.TransferSenderId AS FundingEmployerAccountId,
    ALE.Name AS LegalEntityName,
    Com.AccountLegalEntityId,
    Apps.TrainingCode,
    Apps.TrainingCourseVersion,
    Apps.StartDate,
    Apps.EndDate,
    Apps.TrainingPrice,
    Apps.EndPointAssessmentPrice,
    Apps.Cost AS TotalPrice,
    -1 AS FundingBandMaximum,
    CAST(0 AS BIT) AS DoNotImport

FROM [SFA.DAS.Commitments.Database].dbo.Apprenticeship AS Apps
LEFT JOIN [SFA.DAS.Commitments.Database].dbo.Commitment AS Com ON Apps.CommitmentId = Com.Id
LEFT JOIN [SFA.DAS.Commitments.Database].dbo.AccountLegalEntities AS ALE ON Com.AccountLegalEntityId = ALE.Id
WHERE
    Apps.EndDate > @StartOfAcademicYear AND
    Apps.StartDate < @EndOfAcademicYear AND
    Apps.IsOnFlexiPaymentPilot = 0 AND
    (Apps.StopDate IS NULL OR Apps.StopDate > @StartOfAcademicYear) AND
	Apps.IsApproved = 1


