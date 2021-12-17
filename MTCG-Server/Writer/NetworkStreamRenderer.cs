namespace MTCG_Server.Writer
{
    using MTCG_Server.Handler.ResponseHandler;
    using System.Collections;
    using System.Linq;
    using System.Net.Sockets;
    using System.Text;
    public class NetworkStreamRenderer : IRenderer
    {
        public void Render(object data)
        {
            var values  = (ArrayList)data;
            HttpResponse response = (HttpResponse)values[0];
            NetworkStream stream = (NetworkStream)values[1];

            if (response.Content == null)
            {
                response.Content = new byte[] { };
            }

            if (!response.Headers.ContainsKey("Content-Type"))
            {
                response.Headers["Content-Type"] = "application/json";
            }

            response.Headers["Content-Length"] = response.Content.Length.ToString();

            Write(stream, string.Format("HTTP/1.0 {0} {1}\r\n", ((int)response.Status).ToString(), response.ReasonPhrase));
            Write(stream, string.Join("\r\n", response.Headers.Select(x => string.Format("{0}: {1}", x.Key, x.Value))));
            Write(stream, "\r\n\r\n");

            stream.Write(response.Content, 0, response.Content.Length);
        }

        private static void Write(NetworkStream stream, string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            stream.Write(bytes, 0, bytes.Length);
        }
    }
}
