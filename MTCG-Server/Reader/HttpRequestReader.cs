namespace MTCG_Server.Reader
{
    using MTCG_Server.Enum;
    using MTCG_Server.Handler.RequestHandler;
    using MTCG_Server.Parser;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Net.Sockets;

    public class HttpRequestReader : IReader<HttpRequest>
    {
        public HttpRequestReader(NetworkStream stream)
        {
            this.NetworkStream = stream;
        }

        public NetworkStream NetworkStream
        {
            get;
            private set;
        }

        public HttpRequest Read()
        {
            try
            {
                IReader<string> reader = new NetworkStreamReader(this.NetworkStream);
                string line = reader.Read();

                if (line == null)
                {
                    throw new InvalidOperationException("Invalid request exception");
                }

                #region method

                IParser<HttpMethod> methodParser = new HttpMethodParser();
                HttpMethod httpMethod = methodParser.Parse(line);

                #endregion

                #region path

                IParser<string> pathParser = new PathParser();
                string path = pathParser.Parse(line);

                #endregion

                #region version

                IParser<string> protocolVersionParser = new HttpProtocolVersionParser();
                string protocolVersion = protocolVersionParser.Parse(line);

                #endregion

                #region headers

                IParser<Dictionary<string, string>> headerParser = new HttpHeaderParser();
                Dictionary<string, string> headers = headerParser.Parse(this.NetworkStream);

                #endregion

                #region contentType

                IParser<string> contentTypeParser = new HttpRequestContentTypeParser();
                string contentType = contentTypeParser.Parse(headers);

                #endregion

                #region url

                IParser<string> urlParser = new URLParser();
                string url = urlParser.Parse(line);

                #endregion

                #region content

                IParser<string> contentParser = new HttpRequestContentParser();
                string content = contentParser.Parse(new ArrayList { httpMethod, url, headers, this.NetworkStream });

                #endregion

                #region Auth

                IParser<string> authValueParser = new HttpAuthorizationValueParser();
                string authValue = authValueParser.Parse(headers);

                #endregion

                if (authValue == null)
                    return new HttpRequest(path, url, content, contentType, protocolVersion, httpMethod, headers);
                return new HttpRequest(authValue, path, url, content, contentType, protocolVersion, httpMethod, headers);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }

            return null;
        }
    }
}
