USE [recipesuccess]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[spSearchRecipes]
		@PageNumber = 1,
		@PageSize = 10,
		@SearchVal = N'c'

SELECT	@return_value as 'Return Value'

GO
