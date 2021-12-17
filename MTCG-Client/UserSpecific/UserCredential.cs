using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Client.UserSpecific
{
    public class UserCredential
    {
        private string _name;
        private string _password;

        public UserCredential(string name, string password)
        {
            this.Username = name;
            this.Password = password;
        }

        public string Username
        {
            get
            {
                return this._name;
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Name cant be null or empty");
                }

                this._name = value;
            }
        }
        public string Password
        {
            get
            {
                return this._password;
            }
            set
            {
                if (value == null)
                {
                    throw new NullReferenceException("Password cant be null or empty");
                }

                this._password = value;
            }
        }
    }
}
