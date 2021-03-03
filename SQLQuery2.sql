DECLARE @email varchar(255)
SET @email = '10650700@uvu.edu'

SELECT TOP (1) p.password FROM passwords p 
                             INNER JOIN passwords_users pu ON pu.password_id = p.password_id 
                             INNER JOIN users u ON pu.user_id = u.user_id WHERE (u.email = @email) ORDER BY p.password_changed DESC