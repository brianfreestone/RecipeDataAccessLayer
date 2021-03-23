DECLARE @recipeID int = 3

SELECT r.name, r.created, r.description, mt.name, i.name FROM recipe r 
INNER JOIN meal_type mt ON mt.meal_type_id = r.meal_type_id 
INNER JOIN ingredient i ON r.recipe_id = i.ingredient_id
WHERE r.recipe_id = @recipeID