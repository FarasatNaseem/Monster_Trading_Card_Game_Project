using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.DB
{
    public class CardSchema
    {
        public CardSchema(string id, string name, string damage)
        {
            this.ID = id;
            this.Name = name;
            this.Damage = damage;
        }

      
        public string ID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Damage
        {
            get;
            set;
        }
    }
}
