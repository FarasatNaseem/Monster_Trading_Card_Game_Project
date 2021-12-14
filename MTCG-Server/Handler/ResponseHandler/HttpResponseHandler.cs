using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.Handler.ResponseHandler
{
    public class HttpResponseHandler : NetworkHandler
    {
        public HttpResponseHandler(NetworkStream stream) : base(stream) { }

        protected override void Handle()
        {
            throw new NotImplementedException();
        }
    }
}
