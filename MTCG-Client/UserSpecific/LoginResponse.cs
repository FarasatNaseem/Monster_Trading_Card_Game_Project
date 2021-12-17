namespace MTCG_Client.UserSpecific
{
    public class LoginResponse : Response
    {
        public LoginResponse(string statusCode, string content, string token) : base(statusCode, content)
        {
            this.Token = token;
        }

        public string Token
        {
            get;
            private set;
        }
    }
}
