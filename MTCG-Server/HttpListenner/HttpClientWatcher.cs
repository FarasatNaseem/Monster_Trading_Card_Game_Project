namespace MTCG_Server.HttpListenner
{
    using System;
    using System.Net;
    using System.Net.Sockets;
    using System.Threading;
    public class HttpClientWatcher
    {
        private Thread watcherThread;
        private HttpClientWatcherThreadArgs watcherThreadArgs;
        private HttpClientWatcherWatchAddress watcherWatchAddress;
        private TcpListener listener;

        public HttpClientWatcher(HttpClientWatcherWatchAddress clientWatcherWatchAddress)
        {
            this.watcherWatchAddress = clientWatcherWatchAddress;
            this.listener = new TcpListener(IPAddress.Parse("127.0.0.1"), this.watcherWatchAddress.Port);
            this.watcherThreadArgs = new HttpClientWatcherThreadArgs();
        }

        public event EventHandler<HttpClientWatcherOnClientConnectedEventArgs> OnClientConnected;

        public void Start()
        {
            if (this.watcherThread != null && this.watcherThread.IsAlive)
            {
                throw new InvalidOperationException("HttpClientWatcher thread is already running!");
            }

            this.watcherThreadArgs.Exit = false;
            this.watcherThread = new Thread(this.Watch);
            this.watcherThread.Start(this.watcherThreadArgs);
        }

        public void Stop()
        {
            this.watcherThreadArgs.Exit = true;
            this.watcherThread.Join();
        }

        protected virtual void FireOnHttpClientConnected(TcpClient client)
        {
            if (this.OnClientConnected == null)
            {
                return;
            }

            this.OnClientConnected(this, new HttpClientWatcherOnClientConnectedEventArgs(client));
        }

        private void Watch(object data)
        {
            if (!(data is HttpClientWatcherThreadArgs))
            {
                throw new InvalidOperationException("object must be the type of HTTPClientWatcherThreadArgument");
            }

            var args = (HttpClientWatcherThreadArgs)data;

            try
            {
                this.listener.Start();

                Console.WriteLine("Waiting for clients...");

                while (!args.Exit)
                {
                    TcpClient client = this.listener.AcceptTcpClient();

                    this.FireOnHttpClientConnected(client);
                }
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }
    }
}
