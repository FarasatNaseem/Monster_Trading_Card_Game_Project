using MTCG_Server.Handler.ClientHandler;
using MTCG_Server.HttpListenner;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server
{
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
            Console.WriteLine($"New client is connected: Ip address is {e.ClientAddress}");
            this.httpClientHandler = new HttpClientHandler(e.Client);
            this.httpClientHandler.Start();
        }
    }
}
