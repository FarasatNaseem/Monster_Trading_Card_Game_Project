namespace MTCG_Server.Controller
{
    using MTCG_Server.Handler.RequestHandler;
    using MTCG_Server.Handler.ResponseHandler;

    public interface IController
    {
        public HttpResponse Control(HttpRequest request);
    }
}
