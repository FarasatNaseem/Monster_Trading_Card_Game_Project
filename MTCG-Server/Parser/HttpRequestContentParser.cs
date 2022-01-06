namespace MTCG_Server.Parser
{
    using MTCG_Server.Enum;
    using MTCG_Server.Serialization;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    public class HttpRequestContentParser : IParser<string>
    {
        private ISerializer serializer;

        public HttpRequestContentParser()
        {
            this.serializer = new JSONSerializer();
        }

        public string Parse(object data)
        {
            string requestContent = null;

            var values = (ArrayList)data;

            var httpMethod = (HttpMethod)values[0];
            string url = values[1].ToString();
            var headers = (Dictionary<string, string>)values[2];
            var stream = (NetworkStream)values[3];

            switch (httpMethod)
            {
                case HttpMethod.GET:

                    requestContent = this.ParseParametersFromRequestURL(url);

                    break;
                case HttpMethod.POST: 

                    requestContent = this.ParseContentFromRequest(headers, stream);

                    if (requestContent == null)
                    {
                        throw new ArgumentException("Invalid post request");
                    }



                    break;
                case HttpMethod.PUT:


                    requestContent = this.ParseContentFromRequest(headers, stream);

                    if (requestContent == null)
                    {
                        throw new ArgumentException("Invalid post request");
                    }

                    break;
                default:
                    break;
            }

            return requestContent;
        }

        private string ParseContentFromRequest(Dictionary<string, string> headers, NetworkStream stream)
        {
            StringBuilder myCompleteMessage = new StringBuilder();

            if (headers.ContainsKey("Content-Length"))
            {
                int totalBytes = Convert.ToInt32(headers["Content-Length"]);
                int bytesLeft = totalBytes;

                if (stream.CanRead)
                {
                    while (stream.DataAvailable)
                    {
                        byte[] buffer = new byte[bytesLeft > 1024 ? 1024 : bytesLeft];

                        int numberOfBytesRead = stream.Read(buffer, 0, buffer.Length);

                        myCompleteMessage.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, numberOfBytesRead));
                    }
                }
            }

            return myCompleteMessage.ToString();
        }

        private string ParseParametersFromRequestURL(string url)
        {
            var content = new Dictionary<string, string>();

            if (url.Contains("?"))
            {
                Uri URL = new Uri(url);
                var query = URL.Query.Replace("?", "");

                var queryValues = query.Split('&').Select(q => q.Split('=')).ToDictionary(k => k[0], v => v[1]);

                foreach (var item in queryValues)
                {
                    content.Add(item.Key, item.Value);
                }

                return this.serializer.Serialize(content);
            }

            return null;
        }
    }
}
