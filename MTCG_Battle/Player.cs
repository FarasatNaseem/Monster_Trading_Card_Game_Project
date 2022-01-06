using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Battle
{
    public class Player
    {
        public string Username
        {
            get;
            private set;
        }
        public List<Card> Deck
        {
            get;
            set;
        }
        private readonly Random randomNumGenerator;

        public Player(string username, List<Card> deck)
        {
            Username = username;
            this.Deck = deck;
            randomNumGenerator = new Random();
            this.PlayerWinningSteak = 0;
            this.Elo = 100;
        }

        public int PlayerWinningSteak
        {
            get;
            set;
        }

        public int Elo
        {
            get;
            set;
        }

        public bool IsDeckEmpty()
        {
            return this.Deck.Count == 0 ? true : false;
        }

        public Card GetRandomCard()
        {
            return Deck[randomNumGenerator.Next(Deck.Count)];
        }

        public void RemoveFromDeck(Card card)
        {
            this.Deck.Remove(card);
        }
    }
}
