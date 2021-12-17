using MTCG_Server.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.Handler.ResponseHandler
{
    public class HttpResponse
    {
        public HttpResponse()
        {
            this.Headers = new Dictionary<string, string>();
        }

        public HttpStatusCode Status
        {
            get;
            set;
        }
        public byte[] Content
        {
            get;
            set;
        }


        public string ReasonPhrase
        {
            get;
            set;
        }

        public Dictionary<string, string> Headers
        {
            get;
            set;
        }

        public string ContentAsUTF8
        {
            set
            {
                this.SetContent(value, encoding: Encoding.UTF8);
            }
        }
        public void SetContent(string content, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.UTF8;
            }

            Content = encoding.GetBytes(content);
        }

        public string Path
        {
            get;
            set;
        }


        public override string ToString()
        {
            return string.Format($"status {this.Status} Content {this.ReasonPhrase}");
        }
    }
}
