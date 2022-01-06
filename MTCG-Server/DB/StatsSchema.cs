namespace MTCG_Server.DB
{
    public class StatsSchema
    {
        public StatsSchema(string token, int elo, int winSteak, int loseSteak, int drawSteak, string winner, string looser, string status)
        {
            this.Token = token;
            this.Elo = elo;
            this.WinSteak = winSteak;
            this.LoseSteak = loseSteak;
            this.DrawSteak = drawSteak;
            this.Winner = winner;
            this.Looser = looser;
            this.Status = status;
        }
       
        public string Token
        {
            get;
            private set;
        }

        public int Elo
        {
            get;
            private set;
        }

        public int WinSteak
        {
            get;
            private set;
        }

        public int LoseSteak
        {
            get;
            private set;
        }

        public int DrawSteak
        {
            get;
            private set;
        }

        public string Looser
        {
            get;
            private set;
        }

        public string Winner
        {
            get;
            private set;
        }

        public string Status
        {
            get;
            private set;
        }
    }
}
