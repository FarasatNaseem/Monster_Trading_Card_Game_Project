namespace MTCG_Server.DB
{
    public class CardSchemaWithUserToken : CardSchema
    {
        public CardSchemaWithUserToken(string id, string name, string damage, string token) : base(id, name, damage)
        {
            this.Token = token;
        }

        public string Token
        {
            get;
            private set;
        }
    }
}
