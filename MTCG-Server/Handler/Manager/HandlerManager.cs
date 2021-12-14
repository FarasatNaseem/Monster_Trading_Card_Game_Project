namespace MTCG_Server.Handler.Manager
{
    using MTCG_Server.Handler.RequestHandler;
    using MTCG_Server.Handler.ResponseHandler;
    using MTCG_Server.Routing;
    using MTCG_Server.Writer;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class HandlerManager
    {
        private HttpRequestHandler requestHandler;
        private IInitializer<List<Route>> initializer;
        private HttpResponseHandler responseHandler;
        private IRenderer renderer;
        private List<Route> routes;
        public HandlerManager(NetworkStream inputStream, NetworkStream outputStream)
        {
            this.requestHandler = new HttpRequestHandler(inputStream);
            this.responseHandler = new HttpResponseHandler(outputStream);
            this.renderer = new NetworkStreamRenderer();
            this.initializer = new RouteInitializer();
        }

        public void Start()
        {
            this.requestHandler.OnRequestReceived += RequestHandlerOnRequestReceived;
            this.routes = this.initializer.Initialize();
            this.requestHandler.Start();
        }

        private void RequestHandlerOnRequestReceived(object sender, HttpRequestHandlerOnRequestReceivedEventArgs e)
        {
            Console.WriteLine(e.HttpClientRequest.Content);
        }
    }
}