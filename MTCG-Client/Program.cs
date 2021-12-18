using MTCG_Client.GameSpecific;
using System;

namespace MTCG_Client
{
    public class Program
    {
        private static void Main(string[] args)
        {
            Game game = new Game();
            game.Start();
        }
    }
}