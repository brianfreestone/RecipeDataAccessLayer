DECLARE @userId int
SET @userId = 13

SELECT users.username, users.first_name, users.last_name, friends.primary_user_id FROM users 
                              INNER JOIN friends ON users.user_id = friends.primary_user_id WHERE(friends.secondary_user_id = @userId AND accepted = 0)