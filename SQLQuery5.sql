DECLARE @password varchar(255)
DECLARE @user_id int

SET @password = 'H@mburg3r'
SET @user_id = 19

SELECT password FROM passwords INNER JOIN passwords_users ON passwords.password_id = passwords_users.password_user_id WHERE user_id = @user_id;