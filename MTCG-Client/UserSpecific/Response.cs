namespace MTCG_Client.UserSpecific
{
    public abstract class Response
    {
        public Response(string statusCode, string content)
        {
            this.StatusCode = statusCode;
            this.Content = content;
        }
        public string StatusCode
        {
            get;
            private set;
        }
        public string Content
        {
            get;
            private set;
        }
    }
}
