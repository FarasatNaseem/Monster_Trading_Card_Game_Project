using NUnit.Framework;
using MTCG_Battle;
using System.Collections.Generic;
using System;
using Moq;

namespace MTCG_Battle.Test
{
    public class BattleTest
    {
        private Battle battle;


        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void CollisionDetectorBattleOnCollisionDetected_Test()
        {
            List<Card> playerADeck = new List<Card>()
            {
                new Spell("Spell", 30, CardElementType.Normal, CardOwner.PlayerA),
                new Monster("Monster", 40, CardElementType.Normal, CardOwner.PlayerA, MonsterCardType.Dragon),
                new Monster("Monster", 50, CardElementType.Fire, CardOwner.PlayerA, MonsterCardType.Knight),
                new Spell("Spell", 60, CardElementType.Water, CardOwner.PlayerA)
            };

            List<Card> playerBDeck = new List<Card>()
            {
                new Spell("Spell", 40, CardElementType.Normal, CardOwner.PlayerA),
                new Monster("Monster", 30, CardElementType.Normal, CardOwner.PlayerA, MonsterCardType.Dragon),
                new Monster("Monster", 60, CardElementType.Fire, CardOwner.PlayerA, MonsterCardType.Knight),
                new Spell("Spell", 50, CardElementType.Water, CardOwner.PlayerA)
            };

            Player playerA = new Player("Farasat", playerADeck);
            Player playerB = new Player("Paul", playerBDeck);

            this.battle = new Battle(playerA, playerB);

            this.battle.CollisionDetectorBattleOnCollisionDetected(null, new OnCollisionDetectedEventArgs(playerADeck[0], playerBDeck[0]));


            if (playerA.PlayerWinningSteak == 1)
            {
                Assert.AreEqual(1, playerA.PlayerWinningSteak);
            }
            else if (playerB.PlayerWinningSteak == 1)
            {
                Assert.AreEqual(1, playerB.PlayerWinningSteak);
            }
            else
            {
                Assert.AreEqual(playerA.PlayerWinningSteak, playerB.PlayerWinningSteak);
            }
        }

        [Test]
        [TestCase(30, 15, 7)]
        public void CalculateDrawCalculateTotalDrawSteak(int totalRound, int playerAWinningSteak, int playerBWinningSteak)
        {
            var battle = new Mock<Battle>().Object;

            int drawSteak = battle.CalculateTotalDrawSteak(totalRound, playerAWinningSteak, playerBWinningSteak);

            Assert.AreEqual(drawSteak, 8);
        }
    }
}
