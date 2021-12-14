namespace MTCG_Server.HttpListenner
{
    using System;
    using System.Net.Sockets;
    public class HttpClientWatcherOnClientConnectedEventArgs : EventArgs
    {
        public HttpClientWatcherOnClientConnectedEventArgs(TcpClient client)
        {
            this.Client = client;
        }

        public TcpClient Client
        {
            get;
            private set;
        }

        public string ClientAddress
        {
            get
            {
                return this.Client.Client.RemoteEndPoint.ToString();
            }
        }
    }
}
