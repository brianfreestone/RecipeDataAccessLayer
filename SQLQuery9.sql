DECLARE @user_id int = 13
DECLARE @password varchar(255) = '6afe488e6f28e3b22f0d146e774f84dfab692e4078b22c0d2b3ed6c13a1935b1'

SELECT username FROM users 
INNER JOIN passwords_users ON passwords_users.user_id = users.user_id 
INNER JOIN passwords ON passwords_users.password_id = passwords.password_id
WHERE users.user_id = @user_id AND passwords.password = @password;