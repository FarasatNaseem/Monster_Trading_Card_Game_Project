using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Battle
{
    public class Spell : Card
    {
        public Spell(string name, double damage, CardElementType type, CardOwner cardStatus) : base(name, damage, type, cardStatus)
        {
        }

        public Spell(int x, int y) : base(x, y) { }
        public Spell(string name) : base(name) { }
    }
}
