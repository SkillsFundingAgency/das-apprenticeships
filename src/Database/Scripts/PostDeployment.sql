/*
Post-deployment script
*/

IF OBJECT_ID('[dbo].[Apprenticeships]', 'U') IS NOT NULL
BEGIN
    DROP TABLE [dbo].[Apprenticeships];
END