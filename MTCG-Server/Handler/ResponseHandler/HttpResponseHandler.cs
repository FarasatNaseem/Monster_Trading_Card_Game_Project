namespace MTCG_Server.Handler.ResponseHandler
{
    using MTCG_Server.Enum;
    using MTCG_Server.Handler.RequestHandler;
    using MTCG_Server.Routing;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text.RegularExpressions;

    public class HttpResponseHandler : NetworkHandler
    {
        public HttpResponseHandler(NetworkStream stream, List<Route> routes, HttpRequest request) : base(stream)
        {
            this.Routes = routes;
            this.Request = request;
        }

        public List<Route> Routes
        {
            get;
            private set;
        }
        public HttpRequest Request
        {
            get;
            private set;
        }

        public void Start()
        {
            this.Handle();
        }

        public event EventHandler<HttpResponseHandlerOnResponseReceivedFoundEventArgs> OnResponseReceived;

        protected virtual void FireOnResponseReceived(HttpResponse response)
        {
            if (this.OnResponseReceived == null)
            {
                return;
            }

            this.OnResponseReceived(this, new HttpResponseHandlerOnResponseReceivedFoundEventArgs(response));
        }

        protected override void Handle()
        {
            List<Route> routes = this.Routes.Where(x => Regex.Match(this.Request.Path.ToLower(), x.Url).Success).ToList();

            if (!routes.Any())
            {
                this.FireOnResponseReceived(new HttpResponse()
                {
                    ReasonPhrase = "Not Found",
                    Status = HttpStatusCode.NotFound,
                    ContentAsUTF8 = "{" +
                    "\n    \"Content\": \"Not Found\"" +
                    $"\"\n    \"Status\": \"{((int)HttpStatusCode.NotFound)}\"" +
                    "\n}",
                    Path = this.Request.Path
                });
            }
            else
            {
                Route route = routes.SingleOrDefault(x => x.Method == this.Request.HttpMethod);

                if (route == null)
                {
                    this.FireOnResponseReceived(new HttpResponse()
                    {
                        ReasonPhrase = "Internal Server Error",
                        Status = HttpStatusCode.NotFound,
                        ContentAsUTF8 = "{" +
                    "\n    \"Content\": \"Internal Server Error\"" +
                    $"\"\n    \"Status\": \"{((int)HttpStatusCode.InternalServerError)}\"" +
                    "\n}",
                        Path = this.Request.Path
                    });
                }
                else
                {
                    this.Request.Route = route;

                    HttpResponse response = route.Callable(this.Request);

                    if (response != null)
                    {
                        this.FireOnResponseReceived(response);
                    }
                }
            }
        }
    }
}
