using MTCG_Server.DB;
using MTCG_Server.Enum;
using MTCG_Server.Handler.RequestHandler;
using MTCG_Server.Handler.ResponseHandler;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.Controller
{
    public class UserController : IController
    {
        public UserController()
        {
            this.DbInstance = new Database();
        }
        public Database DbInstance
        {
            get;
            private set;
        }

        public HttpResponse Control(HttpRequest request)
        {
            if (request.Path == "/users")
                return ControlRegisterUserRequest(request);
            if (request.Path == "/sessions")
                return ControlLoginUserRequest(request);

            foreach (var name in this.DbInstance.FetchAllLoggedInUsername())
            {
                if (request.Path == $"/users/{name}" && request.HttpMethod == HttpMethod.GET)
                    return this.ControlGetUserRequest(request);
                if (request.Path == $"/users/{name}" && request.HttpMethod == HttpMethod.PUT)
                    return this.ControlEditUserRequest(request);
            }


            return new HttpResponse();
        }


        private HttpResponse ControlGetUserRequest(HttpRequest request)
        {
            string content = null;
            // /a/b
            string nameFromPath = request.Path.Split('/')[2];
            string nameFromToken = request.Token.Split(' ')[1].Split('-')[0];

            if (nameFromPath == nameFromToken)
            {
                var user = this.DbInstance.FetchSpecificUser(request.Token);

                content = user != null ? JsonConvert.SerializeObject(user, Formatting.Indented) : "Token cant be empty!";
            }
            else
                content = "Path and token are not matching!";

            int code = ((int)HttpStatusCode.Ok);


            string message = "{";
            message += "\n";
            message += "    \"Content\":";
            message += $" \"{content}\",";
            message += "\n";
            message += "    \"Status\":";
            message += $" \"{code}\"";
            message += "\n";
            message += "}";

            return new HttpResponse()
            {
                ReasonPhrase = HttpStatusCode.Ok.ToString(),
                Status = HttpStatusCode.Ok,
                ContentAsUTF8 = message,
                Path = request.Path
            };
        }

        // In process.
        private HttpResponse ControlEditUserRequest(HttpRequest request)
        {
            string content = null;
            // /a/b
            string nameFromPath = request.Path.Split('/')[2];
            string nameFromToken = request.Token.Split(' ')[1].Split('-')[0];

            if (nameFromPath == nameFromToken)
            {
                content = this.DbInstance.UpdateSpecificUserData(request.Token, request.Content) ? "User data is successfully edited" : "Due to some error user data cant be updated!";
            }
            else
                content = "Path and token are not matching!";

            int code = ((int)HttpStatusCode.Ok);


            string message = "{";
            message += "\n";
            message += "    \"Content\":";
            message += $" \"{content}\",";
            message += "\n";
            message += "    \"Status\":";
            message += $" \"{code}\"";
            message += "\n";
            message += "}";

            return new HttpResponse()
            {
                ReasonPhrase = HttpStatusCode.Ok.ToString(),
                Status = HttpStatusCode.Ok,
                ContentAsUTF8 = message,
                Path = request.Path
            };
        }

        // Working.
        private HttpResponse ControlRegisterUserRequest(HttpRequest request)
        {
            string content = this.DbInstance.Register(request.Content) ? "You are now registered." : "This user has been already taken.";

            int code = ((int)HttpStatusCode.Ok);


            string message = "{";
            message += "\n";
            message += "    \"Content\":";
            message += $" \"{content}\",";
            message += "\n";
            message += "    \"Status\":";
            message += $" \"{code}\"";
            message += "\n";
            message += "}";

            return new HttpResponse()
            {
                ReasonPhrase = HttpStatusCode.Ok.ToString(),
                Status = HttpStatusCode.Ok,
                ContentAsUTF8 = message,
                Path = request.Path
            };
        }

        // Working.
        private HttpResponse ControlLoginUserRequest(HttpRequest request)
        {
            string token = this.DbInstance.Login(request.Content);

            //string message = "{";
            //message += "\n";
            //message += "    \"Content\":";
            //message += $" \"{content}\",";
            //message += "\n";
            //message += "    \"Token\":";
            //message += $" \"{token}\",";
            //message += "\n";
            //message += "    \"Status\":";
            //message += $" \"{code}\"";
            //message += "\n";
            //message += "}";


            return new HttpResponse()
            {
                ReasonPhrase = (token != null) ? HttpStatusCode.Ok.ToString() : HttpStatusCode.Unauthorized.ToString(),
                Status = (token != null) ? HttpStatusCode.Ok : HttpStatusCode.Unauthorized,
                ContentAsUTF8 = "{" +
                "    \"Content\":" +
                $" \"{((token != null) ? "You are now Logged in" : "Invalid username or password")}\"," +
                "\n" +
                "    \"Token\":" +
               $" \"{token}\"," +
                "\n" +
                "    \"Status\":" +
               $" \"{((token != null) ? (int)HttpStatusCode.Ok : (int)HttpStatusCode.Unauthorized)}\"" +
               "\n" +
               "}",
                Path = request.Path
            };
        }
    }
}
