using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MTCG_Battle
{
    public class MonsterVsMonsterBattleRound : IBattleRound
    {
        public double CalculateDamage(Card self, Card other)
        {
            return self.Damage;
        }

        public Tuple<CardOwner, BattleRoundStatus> Process(Card playerACard, Card playerBCard)
        {
            Monster cardA = (Monster)playerACard;
            Monster cardB = (Monster)playerBCard;
            
            if(cardA.MonsterCardType == MonsterCardType.Goblin && cardB.MonsterCardType == MonsterCardType.Dragon)
                return new Tuple<CardOwner, BattleRoundStatus>(CardOwner.PlayerB, BattleRoundStatus.Won);
           

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
