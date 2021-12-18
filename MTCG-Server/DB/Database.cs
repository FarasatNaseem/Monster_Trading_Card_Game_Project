namespace MTCG_Server.DB
{
    using System;
    using Newtonsoft.Json.Linq;
    using Npgsql;
    using System.Collections.Generic;
    using System.Data;
    using Newtonsoft.Json;

    public class Database
    {
        // Finished.
        private IDbConnection Connect()
        {
            return new NpgsqlConnection("Host=localhost;Username=root;Password=root;Database=MTCG");
        }

        // Finished.
        public bool Register(string userSchemaJsonObject)
        {
            using (IDbConnection connection = this.Connect())
            {
                var jObject = JObject.Parse(userSchemaJsonObject);

                UserSchema userSchema = new UserSchema(jObject["Username"].ToString(), jObject["Password"].ToString());

                if (!this.IsUserExist(userSchema.Name))
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();

                    try
                    {
                        cmd.CommandText = "Insert into users values(@name, @token ,@password, @coins)";
                        cmd.Parameters.Add(new NpgsqlParameter("@name", userSchema.Name));
                        cmd.Parameters.Add(new NpgsqlParameter("@token", userSchema.Token));
                        cmd.Parameters.Add(new NpgsqlParameter("@password", BCrypt.Net.BCrypt.HashPassword(userSchema.Password)));
                        cmd.Parameters.Add(new NpgsqlParameter("@coins", userSchema.Coin));
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        // Finished.
        public string Login(string userSchemaJsonObject)
        {
            var jObject = JObject.Parse(userSchemaJsonObject);

            UserSchema userSchema = new UserSchema(jObject["Username"].ToString(), jObject["Password"].ToString());

            if (this.IsUserExist(userSchema.Name) && this.IsUserPasswordValid(userSchema))
            {
                return userSchema.Token;
            }

            return null;
        }

        // Finished.
        public bool CreatePackage(string packageJsonObject, string creatorToken)
        {
            if (this.AddPackage(creatorToken) && this.AddPackageCards(packageJsonObject))
                return true;

            if (this.DeleteLastPackage())
                return false;
            return false;
        }

        // Finished.
        public bool AcquirePackage(string userToken)
        {
            /*
           * 1) Check, if user has enough coins to buy package.
           * 2) Check, if package is available.
           * 3) fetch cards from packagecards table of last package id.
           * 5) store cards in a list
           * 4) Delete package which is just fetched.
           * 5) store cards in usercards table
           * 6) detect 5 coins of user
           */

            if (this.HasEnoughCoins(userToken) && (this.GetLastId("package") - 1) != 0)
            {
                List<CardSchema> cardSchemas = this.FetchCardsFromPackageCardsTable(this.GetLastId("package") - 1);

                if (this.AddUserCards(cardSchemas, userToken) && this.DeleteLastPackage() && this.UpdateUserCoins(userToken))
                {
                    return true;
                }

                return false;
            }

            return false;
        }

        public string FetchAllCardOfSpecificUser(string userToken)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                var cards = new List<CardSchemaWithUserToken>();

                using (IDbConnection connection = this.Connect())
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();

                    cmd.Connection = connection;
                    cmd.CommandText = "Select * from usercards where usertoken=@usertoken";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("@usertoken", userToken));
                    NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        cards.Add(new CardSchemaWithUserToken(reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), reader[1].ToString()));
                    }
                    cmd.Dispose();
                    connection.Close();
                }

                return JsonConvert.SerializeObject(cards);
            }
            return null;
        }

        // Finished.
        private List<CardSchema> FetchCardsFromPackageCardsTable(int packageId)
        {
            var cards = new List<CardSchema>(5);

            using (IDbConnection connection = this.Connect())
            {
                connection.Open();
                IDbCommand cmd = connection.CreateCommand();

                cmd.Connection = connection;
                cmd.CommandText = "Select * from packagecards where pid=@pid";
                cmd.CommandType = CommandType.Text;
                cmd.Parameters.Add(new NpgsqlParameter("@pid", packageId));
                NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                while (reader.Read())
                {
                    cards.Add(new CardSchema(reader[0].ToString(), reader[1].ToString(), reader[2].ToString()));
                }
                cmd.Dispose();
                connection.Close();
            }

            return cards;
        }

        // Finished.
        private bool HasEnoughCoins(string userToken)
        {
            try
            {
                using (IDbConnection connection = this.Connect())
                {
                    connection.Open();

                    IDbCommand cmd = connection.CreateCommand();

                    cmd.Connection = connection;
                    cmd.CommandText = "Select * from users where token=@token";
                    cmd.Parameters.Add(new NpgsqlParameter("@token", userToken));

                    NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (Convert.ToInt32(reader[3].ToString()) >= 5)
                        {
                            return true;
                        }
                    }
                    cmd.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // just to print error on console.
            }

            return false;
        }

        // Finished.
        private bool DeleteLastPackage()
        {
            using (IDbConnection connection = this.Connect())
            {
                try
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "Delete from package where id=@id";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("@id", this.GetLastId("package") - 1));
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }

            return true;
        }

        // Finished.
        private int GetUserCoins(string userToken)
        {
            try
            {
                using (IDbConnection connection = this.Connect())
                {
                    connection.Open();

                    IDbCommand cmd = connection.CreateCommand();

                    cmd.Connection = connection;
                    cmd.CommandText = "Select * from users where token=@token";
                    cmd.Parameters.Add(new NpgsqlParameter("@token", userToken));

                    NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        return Convert.ToInt32(reader[3].ToString());
                    }
                    cmd.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // just to print error on console.
            }

            return 0;
        }

        // Finished.
        private bool UpdateUserCoins(string userToken)
        {
            using (IDbConnection connection = this.Connect())
            {
                try
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "update users set coins=@coins where token=@token";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("@token", userToken));
                    cmd.Parameters.Add(new NpgsqlParameter("@coins", this.GetUserCoins(userToken) - 5));
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    connection.Close();
                }

                return true;
            }
        }

        // Finished.
        private bool AddUserCards(List<CardSchema> cards, string userToken)
        {
            using (IDbConnection connection = this.Connect())
            {
                try
                {
                    connection.Open();

                    foreach (var item in cards)
                    {
                        IDbCommand cmd = connection.CreateCommand();
                        cmd.CommandText = "Insert into usercards values(@id, @usertoken, @cardid, @name, @damage)";
                        cmd.Parameters.Add(new NpgsqlParameter("@id", this.GetLastId("usercards")));
                        cmd.Parameters.Add(new NpgsqlParameter("@usertoken", userToken));
                        cmd.Parameters.Add(new NpgsqlParameter("@cardid", item.ID));
                        cmd.Parameters.Add(new NpgsqlParameter("@name", item.Name));
                        cmd.Parameters.Add(new NpgsqlParameter("@damage", item.Damage));
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    connection.Close();
                }

                return true;
            }
        }

        // Finished.
        private bool AddPackageCards(string packageCards)
        {
            using (IDbConnection connection = this.Connect())
            {
                int id = this.GetLastId("package") - 1;
                try
                {
                    connection.Open();
                    List<CardSchema> cards = JsonConvert.DeserializeObject<List<CardSchema>>(packageCards);

                    foreach (var item in cards)
                    {
                        IDbCommand cmd = connection.CreateCommand();
                        cmd.CommandText = "Insert into packagecards values(@id, @name, @damage, @pid)";
                        cmd.Parameters.Add(new NpgsqlParameter("@id", item.ID.ToString()));
                        cmd.Parameters.Add(new NpgsqlParameter("@name", item.Name));
                        cmd.Parameters.Add(new NpgsqlParameter("@damage", item.Damage.ToString()));
                        cmd.Parameters.Add(new NpgsqlParameter("@pid", id));
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return false;
                }
                finally
                {
                    connection.Close();
                }

                return true;
            }
        }

        // Finished.
        private bool IsUserPasswordValid(UserSchema userSchema)
        {
            bool isMatched = false;
            try
            {
                using (IDbConnection connection = this.Connect())
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();

                    cmd.Connection = connection;
                    cmd.CommandText = "Select * from users where name=@name";
                    cmd.Parameters.Add(new NpgsqlParameter("@name", userSchema.Name));

                    NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        isMatched = BCrypt.Net.BCrypt.Verify(userSchema.Password, reader[2].ToString());
                    }
                    cmd.Dispose();
                    connection.Close();

                    return isMatched;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // just to print error on console.
            }

            return false;
        }

        // Finished.
        private bool IsUserExist(string username)
        {
            try
            {
                using (IDbConnection connection = this.Connect())
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();

                    cmd.Connection = connection;
                    cmd.CommandText = "Select * from users where name=@name";
                    cmd.Parameters.Add(new NpgsqlParameter("@name", username));

                    NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        if (username == reader[0].ToString())
                        {
                            return true;
                        }
                    }
                    cmd.Dispose();
                    connection.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // just to print error on console.
            }

            return false;
        }

        // Finished.
        private bool AddPackage(string creatorToken)
        {
            try
            {
                using (IDbConnection connection = this.Connect())
                {
                    connection.Open();

                    PackageSchema packageSchema = new PackageSchema(this.GetLastId("package"), creatorToken);

                    IDbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "Insert into package values(@id, @creatorToken)";
                    cmd.Parameters.Add(new NpgsqlParameter("@id", packageSchema.ID));
                    cmd.Parameters.Add(new NpgsqlParameter("@creatorToken", packageSchema.CreatorToken));
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    connection.Close();

                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message); // just to print error on console.
                return false;
            }
        }

        // Finished.
        private int GetLastId(string tableName)
        {
            int id = 0;
            using (IDbConnection connection = this.Connect())
            {
                connection.Open();
                IDbCommand cmd = connection.CreateCommand();

                cmd.Connection = connection;
                cmd.CommandText = $"Select id from {tableName}";
                NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                while (reader.Read())
                {
                    id = Convert.ToInt32((reader[0].ToString()));
                }
                cmd.Dispose();
                connection.Close();
                Console.WriteLine(id);
                return id + 1;
            }
        }
    }
}
