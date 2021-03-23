USE [recipesuccess]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[spGetFriendsRecipeIDs]
		@userID = 13

SELECT	@return_value as 'Return Value'

GO
