using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.Reader
{
    public class JSONFIleReader : IReader<string>
    {
        private string filePath = "DB/IDs.json";
        public string Read()
        {
            using (StreamReader r = new StreamReader(filePath))
            {
                 return r.ReadToEnd();
            }
        }
    }
}
