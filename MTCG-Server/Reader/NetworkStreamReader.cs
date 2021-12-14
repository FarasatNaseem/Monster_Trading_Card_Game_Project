namespace MTCG_Server.Reader
{
    using System;
    using System.Net.Sockets;

    public class NetworkStreamReader : IReader<string>
    {
        public NetworkStreamReader(NetworkStream stream)
        {
            this.NetworkStream = stream;
        }

        public NetworkStream NetworkStream
        {
            get;
            private set;
        }

        public string Read()
        {
            int next_char;
            string data = "";
            while (true)
            {
                next_char = this.NetworkStream.ReadByte();

                if (next_char == '\n')
                {
                    break;
                }
                if (next_char == '\r')
                {
                    continue;
                }
                if (next_char == -1)
                {
                    continue;
                };

                data += Convert.ToChar(next_char);
            }
            return data;
        }
    }
}
