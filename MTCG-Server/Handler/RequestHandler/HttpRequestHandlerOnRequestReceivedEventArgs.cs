namespace MTCG_Server.Handler.RequestHandler
{
    using System;
    public class HttpRequestHandlerOnRequestReceivedEventArgs : EventArgs
    {
        public HttpRequestHandlerOnRequestReceivedEventArgs(HttpRequest request)
        {
            this.HttpClientRequest = request;
        }

        public HttpRequest HttpClientRequest
        {
            get;
            private set;
        }
    }
}