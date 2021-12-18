using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.Writer
{
    public class JSONFIleRenderer : IRenderer
    {
        public async void Render(object data)
        {
            await using FileStream createStream = File.Create(@"DB/IDs.json");
        }
    }
}
