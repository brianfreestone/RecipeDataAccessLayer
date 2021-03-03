USE [recipesuccess]
GO

DECLARE	@return_value Int,
		@recipe_id int

EXEC	@return_value = [dbo].[insert_recipe]
		@name = N'Chili',
		@user_id = 12,
		@created = N'',
		@description = N'Yummy',
		@meal_type_id = 3,
		@recipe_id = @recipe_id OUTPUT

SELECT	@recipe_id as N'@recipe_id'

SELECT	@return_value as 'Return Value'

GO
