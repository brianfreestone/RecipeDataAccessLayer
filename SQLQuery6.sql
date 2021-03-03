DECLARE @user_id int

SET @user_id = 12


SELECT r.recipe_id, r.created, m.name AS 'meal_type_name' , r.description, r.meal_type_id, r.name AS 'recipe_name' FROM recipe r 
                              INNER JOIN meal_type m ON r.meal_type_id = m.meal_type_id 
                              WHERE r.user_id = @user_id