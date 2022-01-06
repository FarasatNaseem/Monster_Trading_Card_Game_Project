namespace MTCG_Battle
{
    public class Result
    {
        public Result(string winnerName, string looserName, int playerARoundWinningSteak, int playerBRoundWinningSteak, int playerAElo, int playerBElo, string status, int drawSteak)
        {
            this.WinnerName = winnerName;
            this.LooserName = looserName;
            this.PlayerARoundWinningSteak = playerARoundWinningSteak;
            this.PlayerBRoundWinningSteak = playerBRoundWinningSteak;
            this.DrawSteak = drawSteak;
            this.PlayerAElo = playerAElo;
            this.PlayerBElo = playerBElo;
            this.Status = status;
        }

        public string WinnerName
        {
            get;
            set;
        }
        public string LooserName
        {
            get;
            set;
        }
        public int PlayerAElo
        {
            get;
            set;
        }
        public int PlayerBElo
        {
            get;
            set;
        }
        public int PlayerARoundWinningSteak
        {
            get;
            set;
        }
        public int PlayerBRoundWinningSteak
        {
            get;
            set;
        }

        public int DrawSteak
        {
            get;
            set;
        }

        public string Status
        {
            get;
            set;
        }
    }
}
