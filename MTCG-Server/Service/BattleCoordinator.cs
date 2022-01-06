using MTCG_Battle;
using MTCG_Server.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Server.Service
{
    public class BattleCoordinator
    {
        private UserSchema playerASchema;
        private List<CardSchemaWithUserToken> playerACardsSchema;
        private UserSchema playerBSchema;
        private List<CardSchemaWithUserToken> playerBCardsSchema;
        private Database dBInstance;
        public BattleCoordinator(UserSchema playerASchema, List<CardSchemaWithUserToken> playerACardsSchema, UserSchema playerBSchema, List<CardSchemaWithUserToken> playerBCardsSchema)
        {
            this.playerASchema = playerASchema;
            this.playerACardsSchema = playerACardsSchema;
            this.playerBSchema = playerBSchema;
            this.playerBCardsSchema = playerBCardsSchema;
            this.dBInstance = new Database();
        }

        public Result ProcessBattle()
        {
            Player playerA = new Player(this.playerASchema.Name, this.ConvertCardSchemaIntoOriginalCardObject(this.playerACardsSchema, CardOwner.PlayerA));
            Player playerB = new Player(this.playerBSchema.Name, this.ConvertCardSchemaIntoOriginalCardObject(this.playerBCardsSchema, CardOwner.PlayerB));

            return new Battle(playerA, playerB).Start();
        }

        private List<Card> ConvertCardSchemaIntoOriginalCardObject(List<CardSchemaWithUserToken> cards, CardOwner owner)
        {
            List<Card> playerCards = new List<Card>();
            foreach (var card in cards)
            {
                if (card.Name.Contains("Spell"))
                {
                    if (card.Name.Contains("Fire"))
                        playerCards.Add(new Spell(card.Name, Convert.ToDouble(card.Damage.Replace(',', '.')) / 10, CardElementType.Fire, owner));
                    else if (card.Name.Contains("Regular"))
                        playerCards.Add(new Spell(card.Name, Convert.ToDouble(card.Damage.Replace(',', '.')) / 10, CardElementType.Normal, owner));
                    else if (card.Name.Contains("Water"))
                        playerCards.Add(new Spell(card.Name, Convert.ToDouble(card.Damage.Replace(',', '.')) / 10, CardElementType.Water, owner));
                }
                else if (card.Name.Contains("Dragon"))
                {
                    if (card.Name.Contains("Fire"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Fire, MonsterCardType.Dragon, owner));
                    else if (card.Name.Contains("Regular"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Dragon, owner));
                    else if (card.Name.Contains("Water"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Water, MonsterCardType.Dragon, owner));
                    else
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Dragon, owner));
                }
                else if (card.Name.Contains("Knight"))
                {
                    if (card.Name.Contains("Fire"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Fire, MonsterCardType.Knight, owner));
                    else if (card.Name.Contains("Regular"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Knight, owner));
                    else if (card.Name.Contains("Water"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Water, MonsterCardType.Knight, owner));
                    else
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Knight, owner));
                }
                else if (card.Name.Contains("Goblin"))
                {
                    if (card.Name.Contains("Fire"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Fire, MonsterCardType.Goblin, owner));
                    else if (card.Name.Contains("Regular"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Goblin, owner));
                    else if (card.Name.Contains("Water"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Water, MonsterCardType.Goblin, owner));
                    else
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Goblin, owner));
                }
                else if (card.Name.Contains("Elf"))
                {
                    if (card.Name.Contains("Fire"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Fire, MonsterCardType.Elf, owner));
                    else if (card.Name.Contains("Regular"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Elf, owner));
                    else if (card.Name.Contains("Water"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Water, MonsterCardType.Elf, owner));
                    else
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Elf, owner));
                }
                else if (card.Name.Contains("Kraken"))
                {
                    if (card.Name.Contains("Fire"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Fire, MonsterCardType.Kraken, owner));
                    else if (card.Name.Contains("Regular"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Kraken, owner));
                    else if (card.Name.Contains("Water"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Water, MonsterCardType.Kraken, owner));
                    else
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Kraken, owner));
                }
                else if (card.Name.Contains("Ork"))
                {
                    if (card.Name.Contains("Fire"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Fire, MonsterCardType.Ork, owner));
                    else if (card.Name.Contains("Regular"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Ork, owner));
                    else if (card.Name.Contains("Water"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Water, MonsterCardType.Ork, owner));
                    else
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Ork, owner));
                }
                else if (card.Name.Contains("Troll"))
                {
                    if (card.Name.Contains("Fire"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Fire, MonsterCardType.Troll, owner));
                    else if (card.Name.Contains("Regular"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Troll, owner));
                    else if (card.Name.Contains("Water"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Water, MonsterCardType.Troll, owner));
                    else
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Troll, owner));
                }
                else if (card.Name.Contains("Wizard"))
                {
                    if (card.Name.Contains("Fire"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Fire, MonsterCardType.Wizard, owner));
                    else if (card.Name.Contains("Regular"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Wizard, owner));
                    else if (card.Name.Contains("Water"))
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Water, MonsterCardType.Wizard, owner));
                    else
                        playerCards.Add(this.InitializeMonsterCardObject(card, CardElementType.Normal, MonsterCardType.Wizard, owner));
                }
            }

            return playerCards;
        }

        private Card InitializeMonsterCardObject(CardSchemaWithUserToken card, CardElementType cardElementType, MonsterCardType monsterCardType, CardOwner owner)
        {
            return new Monster(card.Name, Convert.ToDouble(card.Damage.Replace(',', '.')) / 10, cardElementType, owner, monsterCardType);
        }
    }
}
