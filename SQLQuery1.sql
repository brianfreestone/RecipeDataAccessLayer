DECLARE @email varchar(255)
DECLARE @password varchar(255)

SET @email = 'test@gmail.com'
SET @password = '03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4'

SELECT users.user_id, users.username, users.first_name, users.last_name, users.is_admin FROM users 
INNER JOIN passwords_users ON users.user_id = passwords_users.user_id 
INNER JOIN passwords ON passwords_users.password_id = passwords.password_id WHERE users.email = @email AND
               passwords.password = (SELECT TOP (1) p.password FROM passwords p 
                             INNER JOIN passwords_users pu ON pu.password_id = p.password_id 
                             INNER JOIN users u ON pu.user_id = u.user_id WHERE u.email = @email ORDER BY p.password_changed DESC)