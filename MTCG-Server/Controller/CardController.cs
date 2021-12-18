namespace MTCG_Server.Controller
{
    using MTCG_Server.DB;
    using MTCG_Server.Enum;
    using MTCG_Server.Handler.RequestHandler;
    using MTCG_Server.Handler.ResponseHandler;

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
                // Create packages.
                "/packages" => this.ControlCreatePackageRequest(request),
                // Acquire package.
                "/transactions/packages" => this.ControlAcquirePackageRequest(request),
                // Get Cards by specific user token.
                "/cards" => this.ControlGetCardsRequest(request),
                _ => new HttpResponse()
            };
        }

        private HttpResponse ControlGetCardsRequest(HttpRequest request)
        {
            string message = this.DbInstance.FetchAllCardOfSpecificUser(request.Token);
            int code = 0;

            if (message == null)
            {
                string content = "Due to some error package can't be acquired";
                code = ((int)HttpStatusCode.BadRequest);

                message = "{";
                message += "\n";
                message += "    \"Content\":";
                message += $" \"{content}\",";
                message += "\n";
                message += "    \"Status\":";
                message += $" \"{code}\"";
                message += "\n";
                message += "}";
            }
            else
            {
                code = ((int)HttpStatusCode.Ok);
            }

            return new HttpResponse()
            {
                ReasonPhrase = HttpStatusCode.Ok.ToString(),
                Status = HttpStatusCode.Ok,
                ContentAsUTF8 = message,

                Path = request.Path
            };
        }

        private HttpResponse ControlAcquirePackageRequest(HttpRequest request)
        {
            string content = this.DbInstance.AcquirePackage(request.Token) ? "Package is acquired" : "Due to some error package can't be acquired";

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
