namespace MTCG_Server.Handler.ResponseHandler
{
    public class HttpResponseHandlerOnResponseReceivedFoundEventArgs
    {
        public HttpResponseHandlerOnResponseReceivedFoundEventArgs(HttpResponse response)
        {
            this.Response = response;
        }

        public HttpResponse Response
        {
            get;
            private set;
        }
    }
}