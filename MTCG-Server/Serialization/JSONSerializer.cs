namespace MTCG_Server.Serialization
{
    using System.Collections.Generic;
    using System.Linq;
    public class JSONSerializer : ISerializer
    {
        public string Serialize(object data)
        {
            string serializedData = null;

            if (data is Dictionary<string, string> dictionary)
            {
                serializedData = this.DictionaryToJsonSerializer(dictionary);
            }

            return serializedData;
        }

        private string DictionaryToJsonSerializer(Dictionary<string, string> data)
        {
            var entries = data.Select(d => string.Format("\"{0}\": \"{1}\"", d.Key, string.Join(",", d.Value)));

            return "{" + string.Join(",", entries) + "}";
        }
    }
}
