namespace MTCG_Server.Handler
{
    using System.Net.Sockets;

    public abstract class NetworkHandler
    {
        public NetworkHandler(NetworkStream stream)
        {
            this.Stream = stream;
        }

        public NetworkStream Stream
        {
            get;
            private set;
        }

        protected abstract void Handle();
    }
}
