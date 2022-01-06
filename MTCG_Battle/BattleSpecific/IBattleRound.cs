namespace MTCG_Battle
{
    using System;

    public interface IBattleRound
    {
        Tuple<CardOwner, BattleRoundStatus> Process(Card playerACard, Card playerBCard);
        double CalculateDamage(Card self, Card other);
    }
}
