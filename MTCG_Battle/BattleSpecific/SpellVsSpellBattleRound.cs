using System;

namespace MTCG_Battle
{
    public class SpellVsSpellBattleRound : IBattleRound
    {
        public double CalculateDamage(Card self, Card other)
        {
            if (self.ElementType == CardElementType.Fire && other.ElementType == CardElementType.Water)
                return (self.Damage / 2);
            if (self.ElementType == CardElementType.Water && other.ElementType == CardElementType.Fire)
                return (self.Damage * 2);

            return self.Damage;
        }


        public Tuple<CardOwner, BattleRoundStatus> Process(Card playerACard, Card playerBCard)
        {

            double playerADamage = this.CalculateDamage(playerACard, playerBCard);
            double playerBDamage = this.CalculateDamage(playerBCard, playerACard);

            if (playerADamage.CompareTo(playerBDamage) > 0)
            {
                return new Tuple<CardOwner, BattleRoundStatus>(CardOwner.PlayerA, BattleRoundStatus.Won);
            }
            if (playerADamage.CompareTo(playerBDamage) < 0)
            {
                return new Tuple<CardOwner, BattleRoundStatus>(CardOwner.PlayerB, BattleRoundStatus.Won);
            }

            return new Tuple<CardOwner, BattleRoundStatus>(CardOwner.None, BattleRoundStatus.Draw);
        }
    }
}
