-- Variables for output
DECLARE @commitmentsDb NVARCHAR(100) = 'Not tested';
DECLARE @apprenticeshipsDb NVARCHAR(100) = 'Not tested';

-- DB 1 Test
BEGIN TRY
    USE [SFA.DAS.Commitments.Database]; -- Replace with your first database name
    CREATE TABLE #TestWriteAccess1 (TestCol INT);
    INSERT INTO #TestWriteAccess1 (TestCol) VALUES (1);
    SET @commitmentsDb = 'Read/Write access confirmed';
    DROP TABLE #TestWriteAccess1;
END TRY
BEGIN CATCH
    SET @commitmentsDb = 'Access failed: ' + ERROR_MESSAGE();
END CATCH;

-- DB 2 Test
BEGIN TRY
    USE [SFA.DAS.Apprenticeships.Database]; -- Replace with your second database name
    CREATE TABLE #TestWriteAccess2 (TestCol INT);
    INSERT INTO #TestWriteAccess2 (TestCol) VALUES (1);
    SET @apprenticeshipsDb = 'Read/Write access confirmed';
    DROP TABLE #TestWriteAccess2;
END TRY
BEGIN CATCH
    SET @apprenticeshipsDb = 'Access failed: ' + ERROR_MESSAGE();
END CATCH;

-- Output the results
SELECT 
    'CommitmentsDb' AS DatabaseName, @commitmentsDb AS Status
UNION ALL
SELECT 
    'ApprenticeshipsDb', @apprenticeshipsDb;
