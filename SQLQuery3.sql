DECLARE @id1 VARCHAR(255)

SET @user_id = 12


SELECT count FROM request WHERE secondary_user_id = @id1 AND accepted = false;