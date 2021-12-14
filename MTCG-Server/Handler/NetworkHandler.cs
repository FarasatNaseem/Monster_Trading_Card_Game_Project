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
        //public Action<object, object> OnRequestReceived { get; internal set; }

        protected abstract void Handle();
    }
}
