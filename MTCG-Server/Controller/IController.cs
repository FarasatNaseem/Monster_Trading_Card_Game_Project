namespace MTCG_Server.Controller
{
    using MTCG_Server.DB;
    using MTCG_Server.Handler.RequestHandler;
    using MTCG_Server.Handler.ResponseHandler;

    public interface IController
    {
        Database DbInstance { get; }
        public HttpResponse Control(HttpRequest request);
    }
}
