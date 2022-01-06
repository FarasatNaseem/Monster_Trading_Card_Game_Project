namespace MTCG_Server
{
    using System;
    using System.Net.Sockets;

    class Program
    {
        private static void Main(string[] args)
        {
            Console.CursorVisible = false;
            try
            {
                 new HttpServer(10001).Start();
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.Message);
            }
        }
    }
}
