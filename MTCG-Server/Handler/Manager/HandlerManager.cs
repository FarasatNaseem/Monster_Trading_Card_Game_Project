namespace MTCG_Server.Handler.Manager
{
    using MTCG_Server.Handler.RequestHandler;
    using MTCG_Server.Handler.ResponseHandler;
    using MTCG_Server.Routing;
    using MTCG_Server.Writer;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    using System.Threading.Tasks;

    public class HandlerManager
    {
        private HttpRequestHandler requestHandler;
        private IInitializer<List<Route>> cardRouteInitializer;
        private HttpResponseHandler responseHandler;
        private IRenderer renderer;
        private List<Route> routes;
        public HandlerManager(NetworkStream inputStream, NetworkStream outputStream)
        {
            this.InputStream = inputStream;
            this.OutputStream = outputStream;
            this.cardRouteInitializer = new RouteInitializer();
        }

        public NetworkStream InputStream
        {
            get;
            private set;
        }

        public NetworkStream OutputStream
        {
            get;
            private set;
        }

        

        public void Start()
        {
            this.requestHandler = new HttpRequestHandler(this.InputStream);
            this.requestHandler.OnRequestReceived += RequestHandlerOnRequestReceived;
            this.routes = this.cardRouteInitializer.Initialize();
            this.requestHandler.Start();
        }

        private void RequestHandlerOnRequestReceived(object sender, HttpRequestHandlerOnRequestReceivedEventArgs e)
        {
            this.responseHandler = new HttpResponseHandler(this.OutputStream, this.routes, e.HttpClientRequest);
            this.responseHandler.OnResponseReceived += ResponseHandlerOnResponseReceived;
            this.responseHandler.Start();
        }

        private void Release()
        {
            this.OutputStream.Flush();
            this.OutputStream.Close();
            this.OutputStream = null;
            this.InputStream.Close();
            this.InputStream = null;
        }

        private void ResponseHandlerOnResponseReceived(object sender, HttpResponseHandlerOnResponseReceivedFoundEventArgs e)
        {
            this.renderer = new NetworkStreamRenderer();
            this.renderer.Render(new ArrayList { e.Response, this.OutputStream });
            this.Release();
        }
    }
}