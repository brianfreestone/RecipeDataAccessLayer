﻿USE [recipesuccess]
GO

DECLARE	@return_value Int

EXEC	@return_value = [dbo].[spGetFriendsRecipes]
		@PageNumber = 1,
		@PageSize = 10,
		@UserID = 15

SELECT	@return_value as 'Return Value'

GO
