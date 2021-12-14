namespace MTCG_Server
{
    using System;
    using System.Net.Sockets;

    class Program
    {
        private static void Main(string[] args)
        {
            try
            {
                HttpServer httpServer = new HttpServer(10001);
                httpServer.Start();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }
    }
}
