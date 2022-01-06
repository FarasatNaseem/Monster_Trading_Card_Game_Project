using MTCG_Server.Enum;

namespace MTCG_Server.DB
{
    public class CardSchemaWithUserToken : CardSchema
    {
        public CardSchemaWithUserToken(string id, string name, string damage, string type, string token) : base(id, name, damage, type)
        {
            this.Token = token;
        }

        public string Token
        {
            get;
            private set;
        }

        public override string ToString()
        {
            return $"  'id={this.ID},token={this.Token},name={this.Name},damage={this.Damage}',\n";
        }
    }
}
