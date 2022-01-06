using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Battle
{
    public class Monster : Card
    {
        public Monster(string name, double damage, CardElementType type, CardOwner cardStatus, MonsterCardType monsterCardType) : base(name, damage, type, cardStatus)
        {
            this.MonsterCardType = monsterCardType;
        }

        public MonsterCardType MonsterCardType
        {
            get;
            private set;
        }
    }
}
