namespace MTCG_Server.Handler.RequestHandler
{
    using System.Collections.Generic;
    using MTCG_Server.Enum;
    using MTCG_Server.Routing;

    public class HttpRequest
    {
        public HttpRequest(string path, string url, string content, string contentType, string version, HttpMethod httpMethod, Dictionary<string, string> headers)
        {
            this.Path = path;
            this.URL = url;
            this.Content = content;
            this.ContentType = contentType;
            this.Version = version;
            this.HttpMethod = httpMethod;
            this.Headers = headers;
        }

        public HttpRequest(string token, string path, string url, string content, string contentType, string version, HttpMethod httpMethod, Dictionary<string, string> headers)
            : this(path, url, content, contentType, version, httpMethod, headers)
        {
            this.Token = token;
        }

        public string Path
        {
            get;
            private set;
        }
        public string URL
        {
            get;
            private set;
        }
        public string Content
        {
            get;
            private set;
        }
        public string ContentType
        {
            get;
            private set;
        }
        public string Version
        {
            get;
            private set;
        }
        public HttpMethod HttpMethod
        {
            get;
            private set;
        }
        public Dictionary<string, string> Headers
        {
            get;
            private set;
        }
        public Route Route { get; set; }
        public string Token { get; set; }
    }
}