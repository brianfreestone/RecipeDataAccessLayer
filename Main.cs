using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecipeLibrary
{
    public class DataAccessLayer
    {

        // connection string
        internal static string connString = @"Data Source=INFO4430-RS-DEV\SQLEXPRESS;Initial Catalog=recipesuccess;User ID=website_user;Password=XIO4$ujdC$b9";
        // internal static string connString = @"Data Source=INFO4430-RS-DEV\SQLEXPRESS;Initial Catalog=test_database;User ID=website_user;Password=XIO4$ujdC$b9";

        private static string cmdText;

        /* USAGE

        -----------------------------------------------------------

           SELECT STATEMENT 

           SELECT column1, column2, column3, ...
           FROM tbl_name WHERE column4 = @param1

        ------------------------------------------------------------

           INSERT INTO STATEMENT 

           INSERT INTO table_name (column1, column2, column3, ...)
                           VALUES (@param1, @param2, @param3, ...)

        ------------------------------------------------------------

           UPDATE STATEMENT 

           UPDATE table_name 
           SET column1= @param1, column2 = @param2, column3= @param3
           WHERE column4 = @param4

        -------------------------------------------------------------

           DELETE STATEMENT 

           DELETE FROM table_name 
           WHERE column1 = @param1

        -----------------------------------------------------------*/



        /// <summary>
        /// User Layer for User Class and methods
        /// </summary>
        public class UserLayer
        {

            // User class | matches fields in db
            public class User
            {
                public int user_id { get; set; }
                public string username { get; set; }
                public string password { get; set; }
                public string email { get; set; }
                public string first_name { get; set; }
                public string last_name { get; set; }
                public DateTime created { get; set; }

                // used for administrator functions
                public bool is_admin { get; set; }
            }

            /// <summary>
            /// inserts a user in to the database
            /// accepts User as parameter
            /// </summary>
            /// <param name="user"></param>
            public static int InsertUser(User user)
            {
                int user_id;

                using (SqlConnection con = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("insert_user", con);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@username", user.username);
                    cmd.Parameters.AddWithValue("@email", user.email);
                    cmd.Parameters.AddWithValue("@first_name", user.first_name);
                    cmd.Parameters.AddWithValue("@last_name", user.last_name);
                    cmd.Parameters.AddWithValue("@is_admin", 0);
                    cmd.Parameters.AddWithValue("@password", user.password);

                    cmd.Parameters.Add("@user_id", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;
                    cmd.Parameters.Add("@password_id", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;

                    con.Open();

                    cmd.ExecuteNonQuery();
                    user_id = Convert.ToInt32(cmd.Parameters["@user_id"].Value);

                }

                return user_id;
            }

            public static void UpdateUserDetails(User user)
            {

            }

            public static void ChangeUserPassword(User user)
            {
                int password_id = 0;

                using (SqlConnection con = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("change_password", con);

                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@password", user.password);
                    cmd.Parameters.AddWithValue("@user_id", user.user_id);
                    cmd.Parameters.AddWithValue("@date_changed", DateTime.Now);
                    cmd.Parameters.AddWithValue("@password_id", password_id);

                    con.Open();

                    cmd.ExecuteNonQuery();

                }
            }

            public static bool PasswordExists(string passwordHash, int user_id)
            {

                List<string> listPasswords = new List<string>();
                bool exists = false;

                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT password FROM passwords INNER JOIN passwords_users ON passwords.password_id = passwords_users.password_user_id WHERE user_id = @user_id;";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        listPasswords.Add(rdr["password"].ToString());
                    }
                }

                if (listPasswords.Count > 0)
                {
                    exists = listPasswords.Contains(passwordHash);
                }

                return exists;

            }

            public static bool ExistingPasswordMatches(int user_id, string passwordHash) 
            {
                bool match = false;
                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT username FROM users " +
                              "INNER JOIN passwords_users ON passwords_users.user_id = users.user_id " +
                              "INNER JOIN passwords ON passwords_users.password_id = passwords.password_id " +
                              "WHERE users.user_id = @user_id AND passwords.password = @password;";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@user_id", user_id);
                    cmd.Parameters.AddWithValue("@password", passwordHash);

                    con.Open();

                    match =(cmd.ExecuteScalar().ToString() != null ? true : false);
                }

                return match;
            }

            public static void DeleteUser(int userID)
            {

            }

            public static List<User> GetUsers()
            {
                // TODO Get list of all users

                List<User> listUsers = new List<User>();
                return listUsers;
            }

            public static User GetUserByEmailAndPassword(string email, string passwordHash)
            {
                User user = new User();

                using (SqlConnection con = new SqlConnection(connString))
                {


                    // TODO create sql statement
                    cmdText = "SELECT users.user_id, users.email, users.username, users.first_name, users.last_name, users.is_admin FROM users " +
                               "INNER JOIN passwords_users ON users.user_id = passwords_users.user_id " +
                               "INNER JOIN passwords ON passwords_users.password_id = passwords.password_id WHERE users.email = @email AND " +
                               "passwords.password = (SELECT TOP(1) p.password FROM passwords p " +
                               "INNER JOIN passwords_users pu ON pu.password_id = p.password_id " +
                               "INNER JOIN users u ON pu.user_id = u.user_id WHERE u.email = @email ORDER BY p.password_changed DESC)";

                    using (SqlCommand cmd = new SqlCommand(cmdText, con))
                    {
                        cmd.CommandText = cmdText;
                        cmd.Parameters.AddWithValue("@email", email);
                        cmd.Parameters.AddWithValue("@password", passwordHash);

                        con.Open();


                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                user.user_id = Convert.ToInt32(rdr["user_id"]);
                                user.username = rdr["username"].ToString();
                                user.email = rdr["email"].ToString();
                                user.first_name = rdr["first_name"].ToString();
                                user.last_name = rdr["last_name"].ToString();
                                user.is_admin = Convert.ToBoolean(rdr["is_admin"]);
                            }
                        }
                    }
                }

                return user;
            }

            public static bool UserExistsByEmail(string email)
            {
                using (SqlConnection con = new SqlConnection(connString))
                {


                    cmdText = "SELECT user_id FROM users WHERE email = @email;";

                    using (SqlCommand cmd = new SqlCommand(cmdText, con))
                    {
                        cmd.CommandText = cmdText;
                        cmd.Parameters.AddWithValue("@email", email);

                        con.Open();
                        int id = Convert.ToInt32(cmd.ExecuteScalar());

                        return (id == 0) ? false : true;
                    }
                }
            }

            public static bool UserExistsByUsername(string username)
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    // TODO create sql statement
                    cmdText = "SELECT user_id FROM users WHERE username = @username";

                    using (SqlCommand cmd = new SqlCommand(cmdText, con))
                    {
                        cmd.CommandText = cmdText;
                        cmd.Parameters.AddWithValue("@username", username);

                        con.Open();
                        int id = Convert.ToInt32(cmd.ExecuteScalar());

                        return (id == 0) ? false : true;
                    }
                }
            }

            public static User GetUserByEmail(string email)
            {
                User user = new User();

                using (SqlConnection con = new SqlConnection(connString))
                {
                    // TODO create sql statement
                    cmdText = "";

                    using (SqlCommand cmd = new SqlCommand())
                    {
                        cmd.CommandText = cmdText;
                        cmd.Parameters.AddWithValue("@email", user.email);

                        con.Open();
                        using (SqlDataReader rdr = cmd.ExecuteReader())
                        {
                            while (rdr.Read())
                            {
                                user.user_id = Convert.ToInt32(rdr["user_id"]);
                                user.username = rdr["username"].ToString();
                                user.email = rdr["email"].ToString();
                                user.first_name = rdr["first_name"].ToString();
                                user.last_name = rdr["last_name"].ToString();
                                user.is_admin = Convert.ToBoolean(rdr["is_admin"]);
                            }
                        }
                    }
                }

                return user;
            }

            public static User GetUserByUserID(int userID)
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT username, first_name, last_name FROM users WHERE user_id = @userID";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@userID", userID);

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    User user = new User();

                    while (rdr.Read())
                    {

                        user.user_id = userID;
                        user.first_name = rdr["first_name"].ToString();
                        user.last_name = rdr["last_name"].ToString();
                        user.username = rdr["username"].ToString();
                    }

                    return user;
                }
            }

            public static int GetUserIDByUserName(string username)
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT user_ID from users WHERE username = @username";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@username", username);

                    con.Open();

                    return Convert.ToInt32(cmd.ExecuteScalar());
                }
            }


            public static User GetUserByUsername(string userName)
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT user_id, first_name, last_name FROM users WHERE username = @username";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@username", userName);

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();

                    User user = new User();

                    while (rdr.Read())
                    {

                        user.user_id = Convert.ToInt32(rdr["user_id"]);
                        user.first_name = rdr["first_name"].ToString();
                        user.last_name = rdr["last_name"].ToString();
                        user.username = userName;
                    }

                    return user;
                }
            }


            public static int GetLastUserID()
            {
                // TODO Get ID of last user ID
                int userID = 0;
                return userID;
            }

            public static List<User> LiveSearchUsers(string searchVal)
            {

                List<User> usernames = new List<User>();

                cmdText = "SELECT user_id, username, first_name, last_name FROM users WHERE first_name LIKE @searchVal + '%'";

                using (SqlConnection con = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@searchVal", searchVal);

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        User newUser = new User();
                        newUser.user_id = Convert.ToInt32(rdr["user_id"]);
                        newUser.username = rdr["username"].ToString();
                        newUser.first_name = rdr["first_name"].ToString();
                        newUser.last_name = rdr["last_name"].ToString();
                        usernames.Add(newUser);
                    }
                }

                return usernames;
            }

            /// <summary>
            /// Queries users table to see if user is an administrator
            /// </summary>
            /// <param name="user_id"></param>
            /// <returns></returns>
            public static bool IsAdministrator(int user_id)
            {
                bool isAdmin = false;

                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT is_admin FROM users WHERE user_id = @user_id;";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    con.Open();

                    isAdmin = Convert.ToBoolean(cmd.ExecuteScalar());
                }

                return isAdmin;
            }


        }

        public class IngredientLayer
        {
            public class Ingredient
            {
                public int? ingredient_id { get; set; }
                public int instruction_id { get; set; }
                public int recipe_id { get; set; }
                public string name { get; set; }
                public string measure_value { get; set; }
                public string instruction { get; set; }

            }

            public static List<Ingredient> GetIngredientsByRecipeID(int recipe_id)
            {
                List<Ingredient> ingredients = new List<Ingredient>();
                using (SqlConnection con = new SqlConnection(connString))
                {
                    //cmdText = "SELECT ingredient.name, ingredient.measure_value, recipe_instruction.instruction, ingredient.ingredient_id, recipe_instruction.recipe_instruction_id " +
                    //          "FROM ingredient RIGHT OUTER JOIN " +
                    //          "recipe_instruction ON ingredient.ingredient_id = recipe_instruction.ingredient_id " +
                    //          "WHERE(recipe_instruction.recipe_id = @recipe_id)";

                    cmdText = "SELECT ingredient_id, name, measure_value " +
                              "FROM ingredient " +
                              "WHERE(recipe_id = @recipe_id)";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@recipe_id", recipe_id);

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Ingredient ingredient = new Ingredient()
                        {
                            ingredient_id = Convert.ToInt32(rdr["ingredient_id"]),
                            //instruction_id = Convert.ToInt32(rdr["recipe_instruction_id"]),
                            //instruction = rdr["instruction"].ToString(),
                            //measure_value = rdr["measure_value"].ToString(),
                            name = rdr["name"].ToString(),
                            recipe_id = recipe_id
                        };

                        ingredients.Add(ingredient);
                    }
                }
                return ingredients;
            }

            public static void InsertIngredients(int recipe_id, List<string> ingredients)
            {
                foreach (var ingredient in ingredients)
                {
                    // add ingredient to ingredient table 
                    cmdText = "INSERT ingredient (recipe_id,name) VALUES(@recipe_id, @name);";
                    using (SqlConnection con = new SqlConnection(connString))
                    {
                        SqlCommand cmd = new SqlCommand(cmdText, con);
                        cmd.Parameters.AddWithValue("@recipe_id", recipe_id);

                        cmd.Parameters.AddWithValue("@name", ingredient);

                        con.Open();

                        cmd.ExecuteNonQuery();
                    }

                }

            }

            // add insruction to recipe_instruction table 
            internal static void InsertInstructions(int recipeID, List<string> instructions)
            {
                foreach (string instruction in instructions)
                {

                    cmdText = "INSERT recipe_instruction (recipe_id, instruction) VALUES(@recipe_id, @instruction);";
                    using (SqlConnection con = new SqlConnection(connString))
                    {
                        SqlCommand cmd = new SqlCommand(cmdText, con);
                        cmd.Parameters.AddWithValue("@recipe_id", recipeID);

                        cmd.Parameters.AddWithValue("@instruction", instruction);

                        con.Open();

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }
        public class MealTypeLayer
        {
            public class MealType
            {
                public int meal_type_id { get; set; }
                public string name { get; set; }
            }

            public static List<MealType> GetMealTypes()
            {
                List<MealType> mealTypes = new List<MealType>();
                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT meal_type_id, name FROM meal_type;";
                    SqlCommand cmd = new SqlCommand(cmdText, con);

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        MealType mealType = new MealType();
                        mealType.meal_type_id = Convert.ToInt32(rdr["meal_type_id"]);
                        mealType.name = rdr["name"].ToString();
                        mealTypes.Add(mealType);
                    }
                }
                return mealTypes;
            }
        }

        public class RecipeLayer
        {
            public class Recipe
            {
                public int recipe_id { get; set; }
                public string name { get; set; }
                public string description { get; set; }
                public int? user_id { get; set; }
                public DateTime created { get; set; }
                public int meal_type_id { get; set; }
                public string meal_type_name { get; set; }
                public List<string> Instructions { get; set; }
                public List<string> Ingredients { get; set; }

            }

            public static int InsertRecipe(Recipe recipe)
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand("insert_recipe", con);
                    cmd.CommandType = System.Data.CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@name", recipe.name);
                    cmd.Parameters.AddWithValue("@user_id", recipe.user_id);
                    cmd.Parameters.AddWithValue("@meal_type_id", recipe.meal_type_id);
                    cmd.Parameters.AddWithValue("@created", DateTime.Now);
                    cmd.Parameters.AddWithValue("@description", recipe.description);

                    cmd.Parameters.Add("@recipe_id", System.Data.SqlDbType.Int).Direction = System.Data.ParameterDirection.Output;

                    con.Open();

                    cmd.ExecuteNonQuery();

                    int recipeID = Convert.ToInt32(cmd.Parameters["@recipe_id"].Value);

                    IngredientLayer.InsertIngredients(recipeID, recipe.Ingredients);

                    IngredientLayer.InsertInstructions(recipeID, recipe.Instructions);

                    return recipeID;


                }
            }

            public static void UpdateFavorite(int recipeID, int userID, string favoriteType)
            {
                if (favoriteType == "add")
                {
                    cmdText = "INSERT favorite_recipe VALUES(@recipeID, @userID);";
                }
                else
                {
                    cmdText = "DELETE FROM favorite_recipe WHERE recipe_ID = @recipeID AND user_id = @userID;";
                }

                using (SqlConnection con = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@recipeID", recipeID);
                    cmd.Parameters.AddWithValue("@userID", userID);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }

            }


            public static List<int> GetFavoritesByUserID(int user_id)
            {
                List<int> favs = new List<int>();

                cmdText = "SELECT recipe_id FROM favorite_recipe WHERE user_id = @user_id";

                using (SqlConnection con = new SqlConnection(connString))
                {
                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        favs.Add(Convert.ToInt32(rdr["recipe_id"]));
                    }
                }
                return favs;
            }

            public static void UpdateRecipe(Recipe recipe)
            {

            }

            public static void DeleteRecipe(int recipeID)
            {

            }

            public static List<Recipe> GetRecipesByUserID(int user_id)
            {
                List<Recipe> listRecipes = new List<Recipe>();

                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT r.recipe_id, r.created, m.name AS 'meal_type_name' , r.description, m.meal_type_id AS mealID, r.name AS 'recipe_name' FROM recipe r " +
                              "INNER JOIN meal_type m ON r.meal_type_id = m.meal_type_id " +
                              "WHERE r.user_id = @user_id";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@user_id", user_id);

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        Recipe recipe = new Recipe();
                        recipe.recipe_id = Convert.ToInt32(rdr["recipe_id"]);
                        recipe.description = rdr["description"].ToString();
                        recipe.meal_type_id = Convert.ToInt32(rdr["mealID"]);
                        recipe.meal_type_name = rdr["meal_type_name"].ToString();
                        recipe.name = rdr["recipe_name"].ToString();
                        recipe.created = Convert.ToDateTime(rdr["created"]);
                        recipe.user_id = user_id;

                        listRecipes.Add(recipe);
                    }
                }

                return listRecipes;
            }

            public static List<Recipe> GetAllRecipes()
            {
                // TODO Get list of all recipes
                List<Recipe> listRecipes = new List<Recipe>();
                return listRecipes;
            }

            public static Recipe GetRecipeByRecipeID(int recipeID)
            {
                // TODO Get recipe by recipe ID
                Recipe recipe = new Recipe();
                return recipe;
            }

            public static List<Recipe> GetFriendsRecipesByUserID(int userID, List<int> friendIDs)
            {
                // TODO Get list of all recipes of friends of user
                List<Recipe> listRecipes = new List<Recipe>();
                return listRecipes;
            }
        }

        public class FriendLayer
        {
            public class Friend
            {
                public int requested_id { get; set; }
                public int primary_user_id { get; set; }
                public int secondary_user_id { get; set; }
                public DateTime request_date { get; set; }
                public bool accepted { get; set; }
            }

            public enum FriendState
            {
                None,
                Pending,
                Accepted
            }

            public static void NewFriendRequest(Friend newRequest)
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "INSERT friends VALUES(@primaryId, @secondaryId, @requestDate, @accepted);";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@primaryId", newRequest.primary_user_id);
                    cmd.Parameters.AddWithValue("@secondaryId", newRequest.secondary_user_id);
                    cmd.Parameters.AddWithValue("@requestDate", DateTime.Now);
                    cmd.Parameters.AddWithValue("@accepted", false);

                    con.Open();

                    cmd.ExecuteNonQuery();

                }
            }

            public static FriendState AreFriends(int id1, int id2)
            {

                FriendState newState = new FriendState();
                //newState = FriendState.None;

                bool accepted = false;

                using (SqlConnection con = new SqlConnection(connString))
                {

                    cmdText = "SELECT accepted FROM friends WHERE ((primary_user_id = @id1 AND secondary_user_id = @id2) OR (primary_user_id = @id2 AND secondary_user_id = @id1))";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@id1", id1);
                    cmd.Parameters.AddWithValue("@id2", id2);
                    //cmd.Parameters.AddWithValue("@id3", id2);
                    //cmd.Parameters.AddWithValue("@id4", id1);

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    if (!rdr.HasRows)
                    {
                        newState = FriendState.None;
                    }
                    else
                    {
                        while (rdr.Read())
                        {
                            accepted = Convert.ToBoolean(rdr["accepted"]);
                            if (accepted)
                            {
                                newState = FriendState.Accepted;
                            }
                            else
                            {
                                newState = FriendState.Pending;
                            }
                        }
                    }
                }

                return newState;
            }

            public static int GetFriendRequestCount(int viewing_id)
            {
                int returnID = 0;

                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT count (*) as total FROM friends WHERE secondary_user_id = @secondaryID AND accepted = 0;";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@secondaryID", viewing_id);

                    con.Open();

                    returnID = Convert.ToInt32(cmd.ExecuteScalar());
                }

                return returnID;
            }

            public static List<UserLayer.User> GetFriendRequestsByUserID(int userID)
            {
                List<UserLayer.User> listRequests = new List<UserLayer.User>();

                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "SELECT users.username, users.first_name, users.last_name, friends.primary_user_id FROM users " +
                              "INNER JOIN friends ON users.user_id = friends.primary_user_id WHERE(friends.secondary_user_id = @userId)";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@userID", userID);

                    con.Open();

                    SqlDataReader rdr = cmd.ExecuteReader();
                    while (rdr.Read())
                    {
                        UserLayer.User user = new UserLayer.User();
                        user.first_name = rdr["first_name"].ToString();
                        user.last_name = rdr["last_name"].ToString();
                        user.username = rdr["username"].ToString();
                        user.user_id = Convert.ToInt32(rdr["primary_user_id"]);

                        listRequests.Add(user);
                    }

                }
                return listRequests;
            }

            public static void AcceptFriendRequest(int id1, int id2)
            {
                using (SqlConnection con = new SqlConnection(connString))
                {
                    cmdText = "UPDATE friends SET accepted = 1 WHERE (primary_user_id = @id1 AND secondary_user_id = @id2);";

                    SqlCommand cmd = new SqlCommand(cmdText, con);
                    cmd.Parameters.AddWithValue("@id1", id1);
                    cmd.Parameters.AddWithValue("@id2", id2);

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }

        }
    }

}