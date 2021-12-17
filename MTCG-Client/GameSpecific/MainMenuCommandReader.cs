using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Client.GameSpecific
{
    public class MainMenuCommandReader : IReader<char>
    {
        public char Read()
        {
            Console.Write("\nPlease enter your command: ");
            return char.Parse(Console.ReadLine());
        }
    }
}
