using System;

namespace MTCG_Server.HttpListenner
{
    public class HttpClientWatcherWatchAddress
    {
        public HttpClientWatcherWatchAddress(int port)
        {
            this.Port = port;
        }

        public int Port
        {
            get;
            private set;
        }
    }
}
