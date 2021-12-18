using System;

namespace MTCG_Client.GameSpecific
{
    public class UI
    {
        public void PrintMTCGHeader()
        {
            Console.WriteLine("WELCOME TO THE MONSTER TRADING CARD GAME");
            Console.WriteLine("----------------------------------------\n\n");
        }

        public void PrintMainMenu()
        {
            Console.WriteLine("\n\nL... LOGIN");
            Console.WriteLine("R... REGISTER");
            Console.WriteLine("C... RUN CURL SCRIPT TO TEST APIs");
            Console.WriteLine("Q... QUIT");
        }
    }
}
