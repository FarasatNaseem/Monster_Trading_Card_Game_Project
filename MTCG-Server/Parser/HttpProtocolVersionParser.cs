namespace MTCG_Server.Parser
{
    using System;
    public class HttpProtocolVersionParser : IParser<string>
    {
        public string Parse(object data)
        {
            if (!(data is string))
            {
                throw new ArgumentException("Object must be type of string!");
            }

            string[] tokens = data.ToString().Split(' ');

            if (tokens.Length != 3)
            {
                throw new InvalidOperationException("Invalid http request!");
            }

            return tokens[2];
        }
    }
}
