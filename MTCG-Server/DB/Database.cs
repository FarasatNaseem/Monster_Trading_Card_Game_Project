namespace MTCG_Server.DB
{
    using System;
    using Newtonsoft.Json.Linq;
    using Npgsql;
    using System.Collections.Generic;
    using System.Data;
    using Newtonsoft.Json;
    using BCrypt.Net;
    using System.Linq;

    public class Database
    {
        // Finished.
        public bool Register(string userSchemaJsonObject)
        {
            using (IDbConnection connection = Connect())
            {
                var jObject = JObject.Parse(userSchemaJsonObject);

                UserSchema userSchema = new UserSchema(jObject["Username"].ToString(), jObject["Password"].ToString(), 20);

                if (!this.IsUserExist(userSchema.Token))
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();

                    try
                    {
                        cmd.CommandText = "Insert into users values(@name, @token ,@password, @coins,@bio, @image, @elo)";
                        cmd.Parameters.Add(new NpgsqlParameter("@name", userSchema.Name));
                        cmd.Parameters.Add(new NpgsqlParameter("@token", userSchema.Token));
                        cmd.Parameters.Add(new NpgsqlParameter("@password", BCrypt.HashPassword(userSchema.Password)));
                        cmd.Parameters.Add(new NpgsqlParameter("@coins", userSchema.Coin));
                        cmd.Parameters.Add(new NpgsqlParameter("@bio", "-"));
                        cmd.Parameters.Add(new NpgsqlParameter("@image", "-"));
                        cmd.Parameters.Add(new NpgsqlParameter("@elo", userSchema.Elo));
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

            if (this.IsUserExist(userSchema.Token) && this.IsUserPasswordValid(userSchema.Token, userSchema.Password))
            {
                if (this.SaveUserInSession(userSchema.Token))
                {
                    return userSchema.Token;
                }
            }

            return null;
        }

        // Finished.
        public bool CreatePackage(string packageJsonObject, string userToken)
        {
            if (!this.IsLoggedIn(userToken))
                return false;

            if (this.AddPackage(userToken) && this.AddPackageCards(packageJsonObject))
                return true;

            if (this.DeletePackage(this.GetLastId("package")))
                return false;
            return false;

        }

        // Finished.
        public bool AcquirePackage(string userToken)
        {
            /*
           * 1) Check, if user has enough coins to buy package.
           * 2) Check, if package is available.
           * 3) fetch cards from packagecards table by last package id.
           * 5) store cards in a list
           * 4) Delete package which is just fetched.
           * 5) store cards in usercards table
           * 6) detect 5 coins of user
           */

            if (this.IsLoggedIn(userToken))
            {
                if (this.HasEnoughCoins(userToken) && this.IsPackageAvailable())
                {
                    List<CardSchema> cardSchemas = this.FetchCardsFromPackageCardsTable(this.GetFirstId("package"));

                    if (this.AddUserCards(cardSchemas, userToken) && this.DeletePackage(this.GetFirstId("package")) && this.UpdateUserCoins(userToken))
                    {
                        return true;
                    }

                    return false;
                }
            }

            return false;
        }

        // Finished.
        public List<CardSchemaWithUserToken> FetchAllCardsOfSpecificUser(string tableName, string userToken)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                if (this.IsLoggedIn(userToken))
                {
                    var cards = new List<CardSchemaWithUserToken>();

                    using (IDbConnection connection = Connect())
                    {
                        connection.Open();
                        IDbCommand cmd = connection.CreateCommand();

                        cmd.Connection = connection;
                        cmd.CommandText = $"Select * from {tableName} where usertoken=@usertoken";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new NpgsqlParameter("@usertoken", userToken));
                        NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string cardType = reader[5].ToString();
                            cardType = cardType.Contains("Spell") ? "Spell" : "Monster";
                            cards.Add(new CardSchemaWithUserToken(reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), cardType, reader[1].ToString()));
                        }
                        cmd.Dispose();
                        connection.Close();
                    }

                    return cards;
                }
            }
            return null;
        }

        // Finished.
        public bool ConfigureDeck(string userToken, string cardIDs)
        {
            if (this.IsLoggedIn(userToken))
            {
                if (this.HasCards(userToken, 4))
                {
                    var jsonArray = JArray.Parse(cardIDs);

                    if (jsonArray.Count < 4)
                        return false;

                    foreach (var id in jsonArray)
                    {
                        CardSchema cardSchema = this.FetchUserCardByID(userToken, id.ToString());

                        if (cardSchema == null)
                            return false;
                        if (!this.AddCardInDeck(userToken, cardSchema))
                            return false;
                    }

                    foreach (var id in jsonArray)
                    {
                        if (!this.DeleteUserCard(userToken, id.ToString()))
                            return false;
                    }

                    return true;
                }
            }

            return false;
        }

        // Finished.
        public List<CardSchemaWithUserToken> FetchAllDeckCardsOfSpecificUser(string userToken)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                if (this.IsLoggedIn(userToken))
                {
                    var cards = new List<CardSchemaWithUserToken>();

                    using (IDbConnection connection = Connect())
                    {
                        connection.Open();
                        IDbCommand cmd = connection.CreateCommand();

                        cmd.Connection = connection;
                        cmd.CommandText = "Select * from userdeck where usertoken=@usertoken";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new NpgsqlParameter("@usertoken", userToken));
                        NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            string cardType = reader[5].ToString();
                            cardType = cardType.Contains("Spell") ? "Spell" : "Monster";
                            cards.Add(new CardSchemaWithUserToken(reader[2].ToString(), reader[3].ToString(), reader[4].ToString(), cardType, reader[1].ToString()));
                        }
                        cmd.Dispose();
                        connection.Close();
                    }

                    return cards;
                }
            }
            return null;
        }

        // Finished.
        public UserSchema FetchSpecificUser(string userToken)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                using (IDbConnection connection = Connect())
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();

                    cmd.Connection = connection;
                    cmd.CommandText = "Select * from users where token=@token";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("@token", userToken));
                    NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        return new UserSchema(reader[0].ToString(), reader[2].ToString(), Convert.ToInt32(reader[3].ToString()), reader[4].ToString(), reader[5].ToString(), Convert.ToInt32(reader[6].ToString()));
                    }
                    cmd.Dispose();
                    connection.Close();
                }
            }
            return null;
        }

        // Finished.
        public bool UpdateSpecificUserData(string userToken, string userSchemaJsonObject)
        {
            var jObject = JObject.Parse(userSchemaJsonObject);

            UserSchema userSchema = new UserSchema(jObject["Name"].ToString(), jObject["Bio"].ToString(), jObject["Image"].ToString());

            using (IDbConnection connection = Connect())
            {
                try
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "update users set name=@name, bio=@bio, image=@image where token=@token";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("@token", userToken));
                    cmd.Parameters.Add(new NpgsqlParameter("@name", userSchema.Name));
                    cmd.Parameters.Add(new NpgsqlParameter("@bio", userSchema.Bio));
                    cmd.Parameters.Add(new NpgsqlParameter("@image", userSchema.Image));
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception)
                {
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
        public List<string> FetchAllLoggedInUsername()
        {
            var usernames = new List<string>();

            using (IDbConnection connection = Connect())
            {
                connection.Open();
                IDbCommand cmd = connection.CreateCommand();

                cmd.Connection = connection;
                cmd.CommandText = "Select * from sessionusers";
                cmd.CommandType = CommandType.Text;
                NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                while (reader.Read())
                {
                    string name = reader[0].ToString().Split(' ')[1].Split('-')[0];
                    usernames.Add(name);
                }
                cmd.Dispose();
                connection.Close();

                return usernames;
            }
        }

        public bool AddUserStats(string userToken, StatsSchema statsSchema)
        {
            using (IDbConnection connection = Connect())
            {

                if (!this.IsUserExist(userToken))
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();

                    try
                    {
                        cmd.CommandText = "Insert into statsdata values(@id, @winner, @looser,@winsteak, @elo, @losesteak, @drawsteak, @status, @usertoken)";
                        cmd.Parameters.Add(new NpgsqlParameter("@id", this.AutoIncrement("statsdata")));
                        cmd.Parameters.Add(new NpgsqlParameter("@winner", statsSchema.Winner));
                        cmd.Parameters.Add(new NpgsqlParameter("@looser", statsSchema.Looser));
                        cmd.Parameters.Add(new NpgsqlParameter("@winsteak", statsSchema.WinSteak));
                        cmd.Parameters.Add(new NpgsqlParameter("@elo", statsSchema.Elo));
                        cmd.Parameters.Add(new NpgsqlParameter("@losesteak", statsSchema.LoseSteak));
                        cmd.Parameters.Add(new NpgsqlParameter("@drawsteak", statsSchema.DrawSteak));
                        cmd.Parameters.Add(new NpgsqlParameter("@status", statsSchema.Status));
                        cmd.Parameters.Add(new NpgsqlParameter("@usertoken", userToken));
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

        public List<StatsSchema> FetchUserStats(string userToken)
        {
            if (!string.IsNullOrEmpty(userToken))
            {
                var statsSchema = new List<StatsSchema>();

                using (IDbConnection connection = Connect())
                {
                    try
                    {
                        connection.Open();
                        IDbCommand cmd = connection.CreateCommand();

                        cmd.Connection = connection;
                        cmd.CommandText = "Select * from stats where playertoken=@playertoken";
                        cmd.CommandType = CommandType.Text;
                        cmd.Parameters.Add(new NpgsqlParameter("@playertoken", userToken));
                        NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();
                        cmd.Dispose();
                        while (reader.Read())
                        {
                            //statsSchema.Add(new StatsSchema(Convert.ToInt32(reader[0].ToString()), reader[1].ToString(), reader[2].ToString(), Convert.ToInt32(reader[3].ToString()), reader[4].ToString()));
                        }
                    }
                    catch (Exception)
                    {
                        return null;
                    }
                    finally
                    {
                        connection.Close();
                    }
                }
            }
            return null;
        }
        public List<int> FetchUserScore(string userToken)
        {
            return null;
        }

        // Finished.
        private static IDbConnection Connect()
        {
            return new NpgsqlConnection("Host=localhost;Username=root;Password=root;Database=MTCG");
        }


        // Finished.
        private bool DeleteUserCard(string userToken, string cardId)
        {
            using (IDbConnection connection = Connect())
            {
                try
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "Delete from usercards where cardid=@cardid";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("@cardid", cardId));
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception)
                {
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
        private bool AddCardInDeck(string userToken, CardSchema cardSchema)
        {
            using (IDbConnection connection = Connect())
            {
                connection.Open();

                IDbCommand cmd = connection.CreateCommand();

                try
                {
                    cmd.CommandText = "Insert into userdeck values(@id, @usertoken, @cardid, @name, @damage, @type)";
                    cmd.Parameters.Add(new NpgsqlParameter("@id", this.AutoIncrement("userdeck")));
                    cmd.Parameters.Add(new NpgsqlParameter("@usertoken", userToken));
                    cmd.Parameters.Add(new NpgsqlParameter("@cardid", cardSchema.ID));
                    cmd.Parameters.Add(new NpgsqlParameter("@name", cardSchema.Name));
                    cmd.Parameters.Add(new NpgsqlParameter("@damage", cardSchema.Damage));
                    cmd.Parameters.Add(new NpgsqlParameter("@type", cardSchema.Type));
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    return true;
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
        }

        // Finished.
        private int GetFirstId(string tableName)
        {
            int id = 0;
            using (IDbConnection connection = Connect())
            {
                connection.Open();
                IDbCommand cmd = connection.CreateCommand();

                cmd.Connection = connection;
                cmd.CommandText = $"Select id from {tableName} FETCH FIRST ROW ONLY";
                NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();

                while (reader.Read())
                {
                    id = Convert.ToInt32((reader[0].ToString()));
                }
                cmd.Dispose();
                connection.Close();
                return id;
            }
        }

        // Finished.
        private CardSchema FetchUserCardByID(string userToken, string id)
        {
            return this.FetchAllCardsOfSpecificUser("usercards", userToken).SingleOrDefault(x => x.ID == id);
        }

        // Finished.
        private bool HasCards(string userToken, int requiredQuantityOfCard)
        {
            return this.FetchAllCardsOfSpecificUser("usercards", userToken).Count >= requiredQuantityOfCard ? true : false;
        }

        // Finished.
        private bool IsPackageAvailable()
        {
            return (this.GetLastId("package")) != 0 ? true : false;
        }

        // Finished.
        private List<CardSchema> FetchCardsFromPackageCardsTable(int packageId)
        {
            var cards = new List<CardSchema>(5);

            using (IDbConnection connection = Connect())
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
                    string cardType = reader[4].ToString();
                    cardType = cardType.Contains("Spell") ? "Spell" : "Monster";
                    cards.Add(new CardSchema(reader[0].ToString(), reader[1].ToString(), reader[2].ToString(), cardType));
                }
                cmd.Dispose();
                connection.Close();
            }

            return cards;
        }

        // Finished.
        private bool HasEnoughCoins(string userToken)
        {
            return this.FetchSpecificUser(userToken).Coin >= 5 ? true : false;
        }

        // Finished.
        private bool DeletePackage(int id)
        {
            using (IDbConnection connection = Connect())
            {
                try
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "Delete from package where id=@id";
                    cmd.CommandType = CommandType.Text;
                    cmd.Parameters.Add(new NpgsqlParameter("@id", id));
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                }
                catch (Exception)
                {
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
            return this.FetchSpecificUser(userToken).Coin;
        }

        // Finished.
        private bool UpdateUserCoins(string userToken)
        {
            using (IDbConnection connection = Connect())
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
            using (IDbConnection connection = Connect())
            {
                try
                {
                    connection.Open();

                    foreach (var item in cards)
                    {
                        IDbCommand cmd = connection.CreateCommand();
                        cmd.CommandText = "Insert into usercards values(@id, @usertoken, @cardid, @name, @damage, @type)";
                        cmd.Parameters.Add(new NpgsqlParameter("@id", this.AutoIncrement("usercards")));
                        cmd.Parameters.Add(new NpgsqlParameter("@usertoken", userToken));
                        cmd.Parameters.Add(new NpgsqlParameter("@cardid", item.ID));
                        cmd.Parameters.Add(new NpgsqlParameter("@name", item.Name));
                        cmd.Parameters.Add(new NpgsqlParameter("@damage", item.Damage));
                        cmd.Parameters.Add(new NpgsqlParameter("@type", item.Type));
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
            using (IDbConnection connection = Connect())
            {
                int id = this.GetLastId("package"); // this is a PK of package table (as a FK)
                try
                {
                    connection.Open();
                    List<CardSchema> cards = JsonConvert.DeserializeObject<List<CardSchema>>(packageCards);


                    foreach (var item in cards)
                    {
                        string cardType = item.Name.Contains("Spell") ? "Spell" : "Monster";

                        IDbCommand cmd = connection.CreateCommand();
                        cmd.CommandText = "Insert into packagecards values(@id, @name, @damage, @pid, @type)";
                        cmd.Parameters.Add(new NpgsqlParameter("@id", item.ID.ToString()));
                        cmd.Parameters.Add(new NpgsqlParameter("@name", item.Name));
                        cmd.Parameters.Add(new NpgsqlParameter("@damage", item.Damage.ToString()));
                        cmd.Parameters.Add(new NpgsqlParameter("@pid", id));
                        cmd.Parameters.Add(new NpgsqlParameter("@type", cardType));
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
        private bool IsUserPasswordValid(string userToken, string password)
        {
            return BCrypt.Verify(password, this.FetchSpecificUser(userToken).Password) ? true : false;
        }

        // Finished.
        private bool IsUserExist(string userToken)
        {
            return this.FetchSpecificUser(userToken) != null ? true : false;
        }

        // Finished.
        private bool AddPackage(string creatorToken)
        {
            try
            {
                using (IDbConnection connection = Connect())
                {
                    connection.Open();

                    PackageSchema packageSchema = new PackageSchema(this.AutoIncrement("package"), creatorToken);

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
        private int AutoIncrement(string tableName)
        {
            return this.GetLastId(tableName) + 1;
        }

        // Finished.
        private int GetLastId(string tableName)
        {
            int id = 0;
            using (IDbConnection connection = Connect())
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
                //Console.WriteLine($"Package last id is {id}");
                return id;
            }
        }

        // Finished.
        private bool SaveUserInSession(string userToken)
        {
            using (IDbConnection connection = Connect())
            {
                try
                {
                    connection.Open();

                    IDbCommand cmd = connection.CreateCommand();
                    cmd.CommandText = "Insert into sessionusers values(@usertoken)";
                    cmd.Parameters.Add(new NpgsqlParameter("@usertoken", userToken));
                    cmd.ExecuteNonQuery();
                    cmd.Dispose();
                    return true;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message); // just to print error on console.
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        // Finished.
        private bool IsLoggedIn(string userToken)
        {
            using (IDbConnection connection = Connect())
            {
                try
                {
                    connection.Open();
                    IDbCommand cmd = connection.CreateCommand();

                    cmd.Connection = connection;
                    cmd.CommandText = "Select * from sessionusers where usertoken=@usertoken";
                    cmd.Parameters.Add(new NpgsqlParameter("@usertoken", userToken));
                    NpgsqlDataReader reader = (NpgsqlDataReader)cmd.ExecuteReader();
                    cmd.Dispose();

                    while (reader.Read())
                    {
                        if (userToken == reader[0].ToString())
                        {
                            return true;
                        }
                    }

                }
                catch (Exception)
                {
                    return false;
                }
                finally
                {
                    connection.Close();
                }
            }

            return false;
        }
    }
}