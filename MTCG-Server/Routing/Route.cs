namespace MTCG_Server.Routing
{
    using System;
    using MTCG_Server.Enum;
    using MTCG_Server.Handler.RequestHandler;
    using MTCG_Server.Handler.ResponseHandler;
    public class Route
    {
        public string Url
        {
            get;
            set;
        }
        
        public HttpMethod Method
        {
            get;
            set;
        }

        public Func<HttpRequest, HttpResponse> Callable
        {
            get;
            set;
        }
    }
}
