using NUnit.Framework;
using System;

namespace MTCG_Battle.Test
{
    public class RoundTest
    {
        private IBattleRound battleRound;
        [SetUp]
        public void Setup()
        {
            this.battleRound = new MonsterVsSpellBattleRound();
        }

        [Test]
        public void ProcessRoundDraw_Test()
        {
            Card playerA = new Spell("Spell", 20, CardElementType.Normal, CardOwner.PlayerA);
            Card playerB = new Monster("Monster", 20, CardElementType.Normal, CardOwner.PlayerA, MonsterCardType.Knight);

            Tuple<CardOwner, BattleRoundStatus> roundResult = this.battleRound.Process(playerA, playerB);

            Assert.AreEqual(BattleRoundStatus.Draw, roundResult.Item2);
        }

        [Test]
        public void ProcessRoundWon_Test()
        {
            Card playerA = new Spell("Spell", 20, CardElementType.Fire, CardOwner.PlayerA);
            Card playerB = new Monster("Monster", 20, CardElementType.Water, CardOwner.PlayerA, MonsterCardType.Knight);

            Tuple<CardOwner, BattleRoundStatus> roundResult = this.battleRound.Process(playerA, playerB);

            Assert.AreEqual(BattleRoundStatus.Won, roundResult.Item2);
        }
      

        [Test]
        public void CalculateDamage_ReturnDefaultDamage_Test()
        {
            Card self = new Spell("Spell", 20, CardElementType.Normal, CardOwner.PlayerA);
            Card other = new Monster("Monster", 20, CardElementType.Normal, CardOwner.PlayerA, MonsterCardType.Knight);

            double damage = this.battleRound.CalculateDamage(self, other);

            Assert.AreEqual(damage, self.Damage);
        }

        [Test]
        public void CalculateDamage_ReturnDoubleDamage_Test()
        {
            Card self = new Spell("Spell", 20, CardElementType.Fire, CardOwner.PlayerA);
            Card other = new Monster("Monster", 20, CardElementType.Water, CardOwner.PlayerA, MonsterCardType.Knight);

            double damage = this.battleRound.CalculateDamage(self, other);

            Assert.AreEqual(damage, self.Damage / 2);
        }

        [Test]
        public void CalculateDamage_ReturnHalfDamage_Test()
        {
            Card self = new Spell("Spell", 20, CardElementType.Water, CardOwner.PlayerA);
            Card other = new Monster("Monster", 20, CardElementType.Fire, CardOwner.PlayerA, MonsterCardType.Knight);

            double damage = this.battleRound.CalculateDamage(self, other);

            Assert.AreEqual(damage, self.Damage * 2);
        }
    }
}