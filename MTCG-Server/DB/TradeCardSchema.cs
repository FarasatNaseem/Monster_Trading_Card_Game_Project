using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.DB
{
    public class TradeCardSchema : CardSchema
    {
        public TradeCardSchema(string id, string name, string damage, string type, string cardToTrade) : base(id, name, damage, type)
        {
            this.CardToTrade = cardToTrade;
        }

        public string CardToTrade
        {
            get;
            private set;
        }
    }
}
