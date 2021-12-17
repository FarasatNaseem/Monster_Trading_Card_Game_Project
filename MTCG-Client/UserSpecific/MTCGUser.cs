using MTCG_Client.UserSpecific.Enum;
using MTCG_Client.UserSpecific.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Client.UserSpecific
{
    public class MTCGUser : IUnRegisteredUser, IRegisteredUser, IUser
    {
        public MTCGUser(string role)
        {
            this.Role = (role == "admin") ? Role.Admin : Role.User;
        }

        public UserCredential UserCredential
        {
            get;
            private set;
        }
       

        public Role Role
        {
            get;
            private set;
        }


        public string SignIn(UserCredential userCredential)
        {
            var json = JsonConvert.SerializeObject(userCredential);

            var url = "http://localhost:10001/sessions";
            var client = new HttpClient();

            var webRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = client.Send(webRequest);

            using var reader = new StreamReader(response.Content.ReadAsStream());

            return reader.ReadToEnd();
        }

        public Response Login(UserCredential userCredential)
        {
            var json = JsonConvert.SerializeObject(userCredential);

            var url = "http://localhost:10001/sessions";
            var client = new HttpClient();

            var webRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = client.Send(webRequest);

            using var reader = new StreamReader(response.Content.ReadAsStream());
            string message = reader.ReadToEnd();

            var jObject = JObject.Parse(message);

            return new LoginResponse(jObject["Status"].ToString(), jObject["Content"].ToString(), jObject["Token"].ToString());
        }

        public Response Register(UserCredential userCredential)
        {
            var json = JsonConvert.SerializeObject(userCredential);

            var url = "http://localhost:10001/users";
            var client = new HttpClient();

            var webRequest = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };

            var response = client.Send(webRequest);

            using var reader = new StreamReader(response.Content.ReadAsStream());
            string message = reader.ReadToEnd();

            var jObject = JObject.Parse(message);

            return new RegistrationResponse(jObject["Status"].ToString(), jObject["Content"].ToString());
        }
    }
}
