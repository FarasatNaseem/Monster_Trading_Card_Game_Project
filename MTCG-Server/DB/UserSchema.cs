using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.DB
{
    public class UserSchema
    {
        public UserSchema(string name, string password)
        {
            this.Name = name;
            this.Password = password;
            this.Coin = 20;
        }
        public int Coin
        {
            get;
            set;
        }

        public string Name
        {
            get;
            private set;
        }

        public string Password
        {
            get;
            private set;
        }

        public string Token
        {
            get
            {
                return "Basic " + this.Name + "-mtcgToken";
            }
        }
    }
}
