DECLARE @userID int
SET @userID = 13

SELECT primary_user_id, secondary_user_id FROM friends INNER JOIN users ON friends.WHERE ((primary_user_id = @userID) OR (secondary_user_id = @userID))