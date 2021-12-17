using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.Parser
{
    public class HttpAuthorizationValueParser : IParser<string>
    {
        public string Parse(object data)
        {
            if (!(data is Dictionary<string, string>))
            {
                throw new ArgumentException("Object must be type of dictionary!");
            }
            var headers = (Dictionary<string, string>)data;

            if (headers.ContainsKey("Authorization"))
            {
                return headers["Authorization"];
            }

            return null;
        }
    }
}
