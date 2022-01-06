using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Battle
{
    public class OnScreenUpdateArgs
    {
        public OnScreenUpdateArgs(int oldX, int oldY, int newX, int newY, string cardName, CardOwner status)
        {
            OldX = oldX;
            OldY = oldY;
            NewX = newX;
            NewY = newY;
            CardName = cardName;
            Status = status;
        }
        public int OldX
        {
            get;
            set;
        }
        public int OldY
        {
            get;
            set;
        }
        public int NewX
        {
            get;
            set;
        }
        public int NewY
        {
            get;
            set;
        }
        public string CardName
        {
            get;
            set;
        }
        public CardOwner Status
        {
            get;
            set;
        }
    }
}
