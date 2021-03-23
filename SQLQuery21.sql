DECLARE @userID int

SELECT  recipe.recipe_id, users.user_id
           FROM recipe INNER JOIN
                         users ON recipe.user_id = users.user_id
WHERE        (recipe.user_id IN
                             (SELECT        primary_user_id
                               FROM            (SELECT        friends.secondary_user_id, friends_1.primary_user_id
                                                         FROM            friends CROSS JOIN
                                                                                   friends AS friends_1
                                                         WHERE        (friends.primary_user_id = @userID) AND (friends_1.secondary_user_id = @userID)) AS derivedtbl_1
                               UNION
                               SELECT        secondary_user_id
                               FROM            (SELECT        friends_2.secondary_user_id, friends_1.primary_user_id
                                                         FROM            friends AS friends_2 CROSS JOIN
                                                                                   friends AS friends_1
                                                         WHERE        (friends_2.primary_user_id = @userID) AND (friends_1.secondary_user_id = @userID)) AS derivedtbl_1_1))

