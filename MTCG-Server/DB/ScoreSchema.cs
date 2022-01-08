namespace MTCG_Server.DB
{
    public class ScoreSchema
    {
        public ScoreSchema(int score)
        {
            this.Score = score;
        }

        public int Score
        {
            get;
            private set;
        }
    }
}
