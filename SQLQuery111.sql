DECLARE @user_id int = 13

SELECT r.recipe_id, r.created, m.name AS 'meal_type_name' , r.description, m.meal_type_id AS mealID, r.name AS 'recipe_name' FROM recipe r 
                              INNER JOIN meal_type m ON r.meal_type_id = m.meal_type_id 
                              INNER JOIN favorite_recipe f ON r.user_id = f.user_id 
                              WHERE r.user_id = @user_id 