using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Battle
{
    public class Player
    {
        private string _username;

        public int CardCount => this.Deck.Count;
        public string Username
        {
            get
            {
                return this._username;
            }
            private set
            {
                if(value == null)
                {
                    throw new ArgumentNullException("Username cant be null");
                }

                this._username = value;
            }
        }
        public List<Card> Deck
        {
            get;
            set;
        }
        private readonly Random randomNumGenerator;

        public Player(string username)
        {
            this.Username = username;
        }

        public Player(string username, List<Card> deck) : this(username)
        {
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

        public void AddCardInDeck(Card card)
        {
            this.Deck.Add(card);
        }

        public void DecrementElo(int value)
        {
            this.Elo -= value;
        }

        public void IncrementElo(int value)
        {
            this.Elo += value;
        }

        public void IncrementWinSteak(int value)
        {
            this.PlayerWinningSteak += value;
        }
    }
}
