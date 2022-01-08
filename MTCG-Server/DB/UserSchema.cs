namespace MTCG_Server.DB
{
    public class UserSchema
    {
        public UserSchema(string name, string password)
        {
            this.Name = name;
            this.Password = password;
            this.Elo = 100;
        }

        public UserSchema(string name, string password, int coin) : this(name, password)
        {
            this.Coin = coin;
        }

        public UserSchema(string name, string password, int coin, string bio, string image, int elo) : this(name, password, coin)
        {
            this.Bio = bio;
            this.Image = image;
            this.Elo = elo;
        }

        public UserSchema(string name, string bio, string image)
        {
            this.Name = name;
            this.Bio = bio;
            this.Image = image;
        }

        public string Bio
        {
            get;
            private set;
        }

        public string Image
        {
            get;
            private set;
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

        public int Elo
        {
            get;
            set;
        }

        public string Token
        {
            get;
            set;
        }
    }
}
