using NUnit.Framework;
using MTCG_Battle;
using Moq;
using System.Collections.Generic;

namespace MTCG_Battle.Test
{
    public class PlayerTest
    {

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Username_Test()
        {
            var player = new Player("Farasat");

            string name = player.Username;

            Assert.NotNull(name);
        }

        [Test]
        public void PlayerCardCount_Test()
        {
            var cards = new List<Card>()
            {
                new Mock<Card>().Object,
                new Mock<Card>().Object
            };
            var player = new Player("player", cards);

            var count = player.CardCount;

            Assert.AreEqual(2, count);
        }

        [Test]
        public void PlayerAddAllCardInDeck_Test()
        {
            var cards = new List<Card>()
            {
                new Mock<Card>().Object,
                new Mock<Card>().Object
            };
            var player = new Player("player", cards);

            var count = player.CardCount;

            Assert.AreEqual(2, count);
        }

        [Test]
        public void PlayerAddCardInCard_Test()
        {
            var cards = new List<Card>()
            {
                new Mock<Card>().Object, new Mock<Card>().Object
            };
            var player = new Player("player", cards);

            var initialCount = player.CardCount;
            player.AddCardInDeck(new Mock<Card>().Object);
            var additionCount = player.CardCount;

            Assert.AreEqual(2, initialCount);
            Assert.AreEqual(3, additionCount);
        }

        [Test]
        public void PlayerRemoveFromDeck_Test()
        {
            var card = new Mock<Card>().Object;
            var cards = new List<Card>()
            {
                card, new Mock<Card>().Object
            };
            var player = new Player("player", cards);

            var initialCount = player.CardCount;
            player.RemoveFromDeck(card);
            var removeCount = player.CardCount;

            Assert.AreEqual(2, initialCount);
            Assert.AreEqual(1, removeCount);
        }

        [Test]
        public void PlayerIsDeckEmpty_Test()
        {
            var cards = new List<Card>()
            {
                new Mock<Card>().Object, new Mock<Card>().Object
            };
            
            var player = new Player("player", cards);

            Assert.IsFalse(player.IsDeckEmpty());
        }

        [Test,]
        public void PlayerRandomCard_Test()
        {
            var cards = new List<Card>()
            {
                new Mock<Card>().Object, new Mock<Card>().Object
            };
            var player = new Player("player", cards);

            var card = player.GetRandomCard();

            Assert.Contains(card, cards);
        }
   
        [Test]
        [TestCase(3)]
        public void IncrementElo_Test(int value)
        {
            var player = new Player("player");

            int beforeIncremented = player.Elo;
            player.Elo += value;
            int afterIncremented = player.Elo;

            Assert.AreNotEqual(beforeIncremented, afterIncremented);

        }

        [TestCase(5)]
        public void DecrementElo_Test(int value)
        {
            var player = new Player("player");

            int beforeDecremented = player.Elo;
            player.Elo -= value;
            int afterDecremented = player.Elo;

            Assert.AreNotEqual(beforeDecremented, afterDecremented);
        }
    }
}
