namespace MTCG_Server
{
    using System;
    using MTCG_Server.Handler.ClientHandler;
    using MTCG_Server.HttpListenner;

    public class HttpServer
    {
        private HttpClientWatcher httpClientWatcher;
        private HttpClientHandler httpClientHandler;
        public HttpServer(int port)
        {
            if (port < 1)
            {
                throw new ArgumentException("Port number cant be lower then 1");
            }

            this.httpClientWatcher = new HttpClientWatcher(new HttpClientWatcherWatchAddress(port));
        }

        public void Start()
        {
            Console.WriteLine("Server is now working..");
            this.httpClientWatcher.OnClientConnected += HttpClientWatcherOnClientConnected; ;
            this.httpClientWatcher.Start();
        }

        private void HttpClientWatcherOnClientConnected(object sender, HttpClientWatcherOnClientConnectedEventArgs e)
        {
            this.httpClientHandler = new HttpClientHandler(e.Client);
            this.httpClientHandler.Start();
        }
    }
}
