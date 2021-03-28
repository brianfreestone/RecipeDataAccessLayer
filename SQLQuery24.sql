DECLARE @searchVal varchar(255) = 'ch'

SELECT r.name AS recipeName, r.user_id, r.description, r.created, mt.name as mealType FROM recipe r 
                          INNER JOIN meal_type mt ON r.meal_type_id = mt.meal_type_id WHERE r.name LIKE @searchVal+'%'