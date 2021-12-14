namespace MTCG_Server.Parser
{
    using System;
    using System.Collections.Generic;

    public class HttpRequestContentTypeParser : IParser<string>
    {
        public string Parse(object data)
        {
            if (!(data is Dictionary<string, string>))
            {
                throw new ArgumentException("Object must be type of dictionary!");
            }
            var headers = (Dictionary<string, string>)data;

            if (headers.ContainsKey("Content-Type"))
            {
                return headers["Content-Type"];
            }

            return null;
        }
    }
}
