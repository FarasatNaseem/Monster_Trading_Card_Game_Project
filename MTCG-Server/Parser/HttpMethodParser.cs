using MTCG_Server.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.Parser
{
    public class HttpMethodParser : IParser<HttpMethod>
    {
        public HttpMethod Parse(object data)
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


            return tokens[0] switch
            {
                "GET" => HttpMethod.GET,
                "POST" => HttpMethod.POST,
                "DELETE" => HttpMethod.DELETE,
                "PATCH" => HttpMethod.PATCH,
                "PUT" => HttpMethod.PUT,
                _ => HttpMethod.NONE
            };
        }
    }
}
