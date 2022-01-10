namespace MTCG_Server.DB
{
    public class CardSchema
    {
        public CardSchema(string id, string name, string damage, string type)
        {
            this.ID = id;
            this.Name = name;
            this.Damage = damage;
            this.Type = type;
        }

        public string Type
        {
            get;
            private set;
        }


        public string ID
        {
            get;
            private set;
        }

        public string Name
        {
            get;
            set;
        }

        public string Damage
        {
            get;
            private set;
        }

    }
}
