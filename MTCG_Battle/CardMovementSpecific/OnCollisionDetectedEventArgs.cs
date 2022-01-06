namespace MTCG_Battle
{
    public class OnCollisionDetectedEventArgs
    {
        public OnCollisionDetectedEventArgs(Card playerACard, Card playerBCard)
        {
            this.PlayerACard = playerACard;
            this.PlayerBCard = playerBCard;
        }

        public Card PlayerACard
        {
            get;
            set;
        }
        public Card PlayerBCard
        {
            get;
            set;
        }
    }
}