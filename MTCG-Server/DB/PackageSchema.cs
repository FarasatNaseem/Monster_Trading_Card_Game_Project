namespace MTCG_Server.DB
{
    using System.Collections.Generic;
    public class PackageSchema
    {
        public PackageSchema(int id, string creatorToken)
        {
            this.ID = id;
            this.CreatorToken = creatorToken;
        }
        public PackageSchema(int id, string creatorToken, List<CardSchema> cards) : this(id, creatorToken)
        {
            this.Cards = cards;
        }

        public PackageSchema(int id, string creatorToken, CardSchema card) : this(id, creatorToken)
        {
            this.Card = card;
        }


        public int ID
        {
            get;
            private set;
        }

        public string CreatorToken
        {
            get;
            private set;
        }
        public CardSchema Card
        {
            get;
            private set;
        }
        public List<CardSchema> Cards
        {
            get;
            private set;
        }
    }
}
