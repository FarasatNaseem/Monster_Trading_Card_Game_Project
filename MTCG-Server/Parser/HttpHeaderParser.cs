namespace MTCG_Server.Parser
{
    using MTCG_Server.Reader;
    using System;
    using System.Collections.Generic;
    using System.Net.Sockets;
    public class HttpHeaderParser : IParser<Dictionary<string, string>>
    {
        public Dictionary<string, string> Parse(object data)
        {
            if (!(data is NetworkStream))
            {
                throw new ArgumentException("Object must be type of network stream");
            }

            var stream = (NetworkStream)data;

            IReader<string> reader = new NetworkStreamReader(stream);

            Dictionary<string, string> headers = new Dictionary<string, string>();
            string headerLine;

            while ((headerLine = reader.Read()) != null)
            {
                var values = headerLine.Split(": ");

                if (values.Length == 0 || values.Length == 1)
                {
                    return headers;
                }

                headers.Add(values[0], values[1]);
            }

            return headers;
        }
    }
}
