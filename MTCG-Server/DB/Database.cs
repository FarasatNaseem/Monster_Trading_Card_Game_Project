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
        private static int packageID = 1;
        private IDbConnection Connect()
        {
            return new NpgsqlConnection("Host=localhost;Username=root;Password=root;Database=MTCG");
        }

        public bool Register(string userCredentialJsonObject)
        {
            using (IDbConnection connection = this.Connect())
            {
                var jObject = JObject.Parse(userCredentialJsonObject);

                UserSchema userCredentialSchema = new UserSchema(jObject["Username"].ToString(), jObject["Password"].ToString());

                if (!this.IsUserExist(userCredentialSchema.Name))
                {
                    try
                    {
                        connection.Open();
                        IDbCommand cmd = connection.CreateCommand();

                        cmd.CommandText = "Insert into users values(@name, @token ,@password)";
                        cmd.Parameters.Add(new NpgsqlParameter("@name", userCredentialSchema.Name));
                        cmd.Parameters.Add(new NpgsqlParameter("@token", userCredentialSchema.Token));
                        cmd.Parameters.Add(new NpgsqlParameter("@password", BCrypt.Net.BCrypt.HashPassword(userCredentialSchema.Password)));
                        cmd.ExecuteNonQuery();
                        cmd.Dispose();
                        connection.Close();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                        return false;
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

            UserSchema userCredentialSchema = new UserSchema(jObject["Username"].ToString(), jObject["Password"].ToString());

            if (this.IsUserExist(userCredentialSchema.Name) && this.VerifyPasswordOfUser(userCredentialSchema))
            {
                return userCredentialSchema.Token;
            }

            return null;
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


        public bool AddPackage(string creatorToken)
        {
            try
            {
                using (IDbConnection connection = this.Connect())
                {
                    connection.Open();
                    PackageSchema packageSchema = new PackageSchema(packageID++, creatorToken);

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

        public bool CreatePackage(string packageJsonObject, string creatorToken)
        {
            if (this.AddPackage(creatorToken))
            {
                int packageId = packageID - 1;
                try
                {
                    using (IDbConnection connection = this.Connect())
                    {
                        connection.Open();
                        List<CardSchema> cards = JsonConvert.DeserializeObject<List<CardSchema>>(packageJsonObject);

                        foreach (var item in cards)
                        {
                            IDbCommand cmd = connection.CreateCommand();
                            cmd.CommandText = "Insert into packagecards values(@id, @name, @damage, @pid)";
                            cmd.Parameters.Add(new NpgsqlParameter("@id", item.ID.ToString()));
                            cmd.Parameters.Add(new NpgsqlParameter("@name", item.Name));
                            cmd.Parameters.Add(new NpgsqlParameter("@damage", item.Damage.ToString()));
                            cmd.Parameters.Add(new NpgsqlParameter("@pid", packageId));
                            cmd.ExecuteNonQuery();
                            cmd.Dispose();
                        }

                        connection.Close();
                        return true;
                    }
                }
                catch (Exception ex)
                {
                    return false;
                }

            }
            else
            {
                return false;
            }
        }
    }
}
