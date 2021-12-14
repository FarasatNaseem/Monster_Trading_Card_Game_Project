namespace MTCG_Server.Handler.ClientHandler
{
    using MTCG_Server.Handler.Manager;
    using System.Net.Sockets;
    using System.Threading;

    class HttpClientHandler
    {
        private HandlerManager handlerManager;
        private Thread handlerThread;

        public HttpClientHandler(TcpClient client)
        {
            this.handlerManager = new HandlerManager(client.GetStream(), client.GetStream());
        }

        public void Start()
        {
            this.handlerThread = new Thread(this.handlerManager.Start);
            this.handlerThread.Start();
        }

        public void Stop()
        {
            this.handlerThread.Join();
        }
    }
}
