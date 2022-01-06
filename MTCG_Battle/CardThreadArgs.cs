using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Battle
{
    public class CardThreadArgs
    {
        public CardThreadArgs()
        {
            this.Exit = false;
        }

        public bool Exit
        {
            get;
            set;
        }
    }
}
