using NUnit.Framework;
using MTCG_Battle;

namespace MTCG_Battle.Test
{
    public class CardTest
    {
        private Card card;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CardNameShouldNotBeNullOrEmpty_Test()
        {
            this.card = new Spell("Farasat");

            string name = this.card.Name;

            Assert.NotNull(name);
            Assert.IsNotEmpty(name);
        }

        [Test]
        public void CardMovementDirectionOfPlayerA_Test()
        {
            this.card = new Spell("Card1", 10, CardElementType.Fire, CardOwner.PlayerA);

            Direction direction = this.card.Direction;

            Assert.AreEqual(Direction.Right, direction);
        }

        [Test]
        public void CardMovementDirectionOfPlayerB_Test()
        {
            this.card = new Spell("Card1", 10, CardElementType.Fire, CardOwner.PlayerA);

            Direction direction = Direction.Left;

            Assert.AreEqual(Direction.Left, direction);
        }

        [Test]
        public void MonsterCardType_Test()
        {
            Monster card = new Monster("Monster", 10, CardElementType.Fire, CardOwner.PlayerA, MonsterCardType.Dragon);

            MonsterCardType type = card.MonsterCardType;
            Assert.AreEqual(MonsterCardType.Dragon, type);
        }
    }
}
