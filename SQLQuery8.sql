DECLARE @user_id int
SET @user_id = 12

SELECT recipe_id FROM favorite_recipe WHERE user_id = @user_id
