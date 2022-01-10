using MTCG_Server.Reader;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTCG_Server.Handler.RequestHandler
{
    class HttpRequestHandler : NetworkHandler
    {
        private Thread httpClientHandlerThread;
        private IReader<HttpRequest> reader;
        public HttpRequestHandler(NetworkStream stream) : base(stream) { }

        public event EventHandler<HttpRequestHandlerOnRequestReceivedEventArgs> OnRequestReceived;

        public void Start()
        {
            if (this.httpClientHandlerThread != null && this.httpClientHandlerThread.IsAlive)
            {
                throw new ArgumentException($"HttpRequestThread is already running");
            }

            this.httpClientHandlerThread = new Thread(this.Handle);
            this.httpClientHandlerThread.Start();
        }
        public void Stop()
        {
            this.httpClientHandlerThread.Join();
        }

        protected virtual void FireOnReceivedRequest(HttpRequest httpRequest)
        {
            if (this.OnRequestReceived == null)
            {
                return;
            }

            this.OnRequestReceived(this, new HttpRequestHandlerOnRequestReceivedEventArgs(httpRequest));
        }

        protected override void Handle()
        {
            if (this.Stream.CanRead && this.Stream.DataAvailable)
            {
                // New request.
                this.reader = new HttpRequestReader(this.Stream);
                HttpRequest httpRequest = this.reader.Read();

                if (httpRequest != null)
                {
                    this.FireOnReceivedRequest(httpRequest);
                }
            }

            this.Stop();
        }
    }
}
