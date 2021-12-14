namespace MTCG_Server.Parser
{
    using System;
    public class PathParser : IParser<string>
    {
        public string Parse(object data)
        {
            if (!(data is string))
            {
                throw new ArgumentException("Object must be the type of string!");
            }

            string[] tokens = data.ToString().Split(' ');

            if (tokens.Length != 3)
            {
                throw new InvalidOperationException("Invalid http request!");
            }


            if (tokens[1].Contains("?"))
            {
                int index = tokens[1].LastIndexOf('?');

                return tokens[1].Substring(0, index);
            }

            return tokens[1];
        }
    }
}
