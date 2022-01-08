using NUnit.Framework;
using MTCG_Battle;

namespace MTCG_Battle.Test
{
    public class CardCollisionDetectorTest
    {
        private CardCollisionDetector cardCollisionDetector;
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase(5, 5, 5, 5)]
        public void IsCollied_Test(int playerAX, int playerBX, int playerAY, int playerBY)
        {
            Card spell = new Spell(playerAX, playerAY);
            Card monster = new Monster(playerBX, playerBY);

            this.cardCollisionDetector = new CardCollisionDetector(spell, monster);
            
            Assert.IsTrue(this.cardCollisionDetector.IsCollied(spell.X, monster.X, spell.Y, monster.Y));
        }
    }
}
