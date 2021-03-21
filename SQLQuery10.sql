USE [recipesuccess]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[spGetEmployees]
		@PageNumber = 1,
		@PageSize = 10,
		@UserID = 13

SELECT	@return_value as 'Return Value'

GO
