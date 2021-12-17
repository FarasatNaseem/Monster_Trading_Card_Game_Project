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
    public class CardController : IController
    {
        public CardController()
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
                // Create packages
                "/packages" => ControlCreatePackageRequest(request),
                _ => new HttpResponse()
            };
        }

        private HttpResponse ControlCreatePackageRequest(HttpRequest request)
        {
            string content = this.DbInstance.CreatePackage(request.Content, request.Token) ? "Packages are created" : "Due to some error packages can't be created";

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

    }
}
