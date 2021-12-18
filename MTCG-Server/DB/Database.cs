using Newtonsoft.Json.Linq;
using Npgsql;
using NpgsqlTypes;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Xml.Serialization;
using BCrypt.Net;
using Newtonsoft.Json;

namespace MTCG_Server.DB
{
    public class Database
    {
        private IDbConnection Connect()
        {
            return new NpgsqlConnection("Host=localhost;Username=root;Password=root;Database=MTCG");
        }

        public bool Register(string userCredentialJsonObject)
        {
            using (IDbConnection connection = this.Connect())
            {
                var jObject = JObject.Parse(userCredentialJsonObject);

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

        public string Login(string userCredentialJsonObject)
        {
            var jObject = JObject.Parse(userCredentialJsonObject);

            UserSchema userSchema = new UserSchema(jObject["Username"].ToString(), jObject["Password"].ToString());

            if (this.IsUserExist(userSchema.Name) && this.VerifyPasswordOfUser(userSchema))
            {
                return userSchema.Token;
            }

            return null;
        }

        public bool CreatePackage(string packageJsonObject, string creatorToken)
        {
            if (this.AddPackage(creatorToken) && this.AddPackageCards(packageJsonObject))
                return true;

            if (this.DeleteInvalidIDFromPackage())
                return false;
            return false;
        }

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

            // 1
            if (this.HasEnoughCoins(userToken))
            {
                // 2
                if (this.GetLastPackageId() != 0)
                {
                    return true;
                }

                return false;
            }

            return true;
        }


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
        private bool DeleteInvalidIDFromPackage()
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
                    cmd.Parameters.Add(new NpgsqlParameter("@id", this.GetLastPackageId() - 1));
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
        private bool AddPackageCards(string packageCards)
        {
            using (IDbConnection connection = this.Connect())
            {
                int id = this.GetLastPackageId();
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
                    return false;
                }
                finally
                {
                    connection.Close();
                }

                return true;
            }
        }
        private bool VerifyPasswordOfUser(UserSchema userSchema)
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
        private bool AddPackage(string creatorToken)
        {
            try
            {
                using (IDbConnection connection = this.Connect())
                {
                    connection.Open();

                    PackageSchema packageSchema = new PackageSchema(this.GetLastPackageId() + 1, creatorToken);

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
        private int GetLastPackageId()
        {
            int id = 0;
            using (IDbConnection connection = this.Connect())
            {
                connection.Open();
                IDbCommand cmd = connection.CreateCommand();

                cmd.Connection = connection;
                cmd.CommandText = "Select id from package";
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
    }
}
