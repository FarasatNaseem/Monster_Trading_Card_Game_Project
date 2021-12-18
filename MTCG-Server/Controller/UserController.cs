using MTCG_Server.DB;
using MTCG_Server.Enum;
using MTCG_Server.Handler.RequestHandler;
using MTCG_Server.Handler.ResponseHandler;
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
            return request.Path switch
            {
                // Register
                "/users" => ControlRegisterUserRequest(request),
                // Login
                "/sessions" => ControlLoginUserRequest(request),
                _ => new HttpResponse()
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
                //"{" +
                //   $"\"\n    \"Content\": \"{content}" +
                //    $"\"\n    \"Status\": \"{code}\"" +
                //    "\n}",
                Path = request.Path
            };
        }

        // Working.
        private HttpResponse ControlLoginUserRequest(HttpRequest request)
        {
            string token = this.DbInstance.Login(request.Content);
            string content = (token != null) ? "You are now Logged in" : "Invalid username or password";
            int code = ((int)HttpStatusCode.Ok);
            string message = "{";
            message += "\n";
            message += "    \"Content\":";
            message += $" \"{content}\",";
            message += "\n";
            message += "    \"Token\":";
            message += $" \"{token}\",";
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
    }
}
