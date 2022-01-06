namespace MTCG_Battle
{
    using System;
    using System.Threading;

    public class Battle
    {
        private object locker = new object();
        private CardCollisionDetector collisionDetector;
        private IBattleRound battleRound;
        private Player playerA;
        private Player playerB;
        private bool isRoundFinished;
        private int totalRound = 30;
        private int currentRound = 0;

        public Battle(Player playerA, Player playerB)
        {
            this.playerA = playerA;
            this.playerB = playerB;
            this.isRoundFinished = true;
        }

        public Result Start()
        {
            int drawSteak = 0;
            Console.Clear();
            int round = totalRound;
            for (int i = 0; i < round; i++)
            {
                if (this.isRoundFinished)
                {
                    this.totalRound -= 1;
                    this.currentRound += 1;
                    if (!this.playerA.IsDeckEmpty() && !this.playerB.IsDeckEmpty())
                    {
                        Card playerACard = this.playerA.GetRandomCard();
                        Card playerBCard = this.playerB.GetRandomCard();
                        playerACard.OnPositionChanged += OnPositionChanged;
                        playerBCard.OnPositionChanged += OnPositionChanged;
                        playerACard.StartPlaying();
                        playerBCard.StartPlaying();
                        this.collisionDetector = new CardCollisionDetector(playerACard, playerBCard);
                        this.collisionDetector.OnCollisionDetected += CollisionDetectorOnCollisionDetected;
                        this.collisionDetector.Start();
                    }

                    if (!this.playerA.IsDeckEmpty() && this.playerB.IsDeckEmpty())
                    {
                        drawSteak = this.CalculateTotalDrawSteak(round, this.playerA.PlayerWinningSteak, this.playerB.PlayerWinningSteak);
                        return new Result(this.playerA.Username, this.playerB.Username, this.playerA.PlayerWinningSteak, this.playerB.PlayerWinningSteak, this.playerA.Elo, this.playerB.Elo, "Won", drawSteak);
                    }
                    if (this.playerA.IsDeckEmpty() && !this.playerB.IsDeckEmpty())
                    {
                        drawSteak = this.CalculateTotalDrawSteak(round, this.playerA.PlayerWinningSteak, this.playerB.PlayerWinningSteak);
                        return new Result(this.playerB.Username, this.playerA.Username, this.playerA.PlayerWinningSteak, this.playerB.PlayerWinningSteak, this.playerA.Elo, this.playerB.Elo, "Won", drawSteak);
                    }

                    isRoundFinished = false;
                }
                else
                {
                    i--;
                    continue;
                }
            }

            while (true)
            {
                if (isRoundFinished)
                {
                    Console.Clear();
                    Console.WriteLine("Game over");
                    break;
                }
            }

            drawSteak = this.CalculateTotalDrawSteak(round, this.playerA.PlayerWinningSteak, this.playerB.PlayerWinningSteak);

            if (this.playerA.Deck.Count > this.playerB.Deck.Count)
            {
                return new Result(this.playerA.Username, this.playerB.Username, this.playerA.PlayerWinningSteak, this.playerB.PlayerWinningSteak, this.playerA.Elo, this.playerB.Elo, "Won", drawSteak);
            }
            if (this.playerB.Deck.Count > this.playerA.Deck.Count)
            {
                return new Result(this.playerB.Username, this.playerA.Username, this.playerA.PlayerWinningSteak, this.playerB.PlayerWinningSteak, this.playerA.Elo, this.playerB.Elo, "Won", drawSteak);
            }

            return new Result(this.playerB.Username, this.playerA.Username, this.playerA.PlayerWinningSteak, this.playerB.PlayerWinningSteak, this.playerA.Elo, this.playerB.Elo, "Draw", drawSteak);
        }

        private int CalculateTotalDrawSteak(int totalRound, int playerAWinningSteak, int playerBWinningSteak)
        {
            return totalRound - (playerAWinningSteak + playerBWinningSteak);
        }

        private void CollisionDetectorOnCollisionDetected(object sender, OnCollisionDetectedEventArgs e)
        {
            Tuple<CardOwner, BattleRoundStatus> roundResult = null;
            double playerACardDamage = e.PlayerACard.Damage;
            double playerBCardDamage = e.PlayerBCard.Damage;

            if (e.PlayerACard is Spell && e.PlayerBCard is Spell)
            {
                roundResult = this.ProcessSpellVsSpellRound(e.PlayerACard, e.PlayerBCard);
            }
            else if (e.PlayerACard is Monster && e.PlayerBCard is Monster)
            {
                roundResult = this.ProcessMonsterVsMonsterRound(e.PlayerACard, e.PlayerBCard);
            }
            else if ((e.PlayerACard is Spell && e.PlayerBCard is Monster) || (e.PlayerACard is Monster && e.PlayerBCard is Spell))
            {
                roundResult = this.ProcessMonsterVsSpellRound(e.PlayerACard, e.PlayerBCard);
            }

            e.PlayerACard.Damage = playerACardDamage;
            e.PlayerBCard.Damage = playerBCardDamage;

            if (roundResult.Item1 == CardOwner.PlayerA && roundResult.Item2 == BattleRoundStatus.Won)
            {
                e.PlayerBCard.Owner = CardOwner.PlayerA;
                this.playerA.Deck.Add(e.PlayerBCard);
                this.playerB.RemoveFromDeck(e.PlayerBCard);
                this.playerA.PlayerWinningSteak++;
                this.playerA.Elo += 3;
                this.playerB.Elo -= 5;
            }
            else if (roundResult.Item1 == CardOwner.PlayerB && roundResult.Item2 == BattleRoundStatus.Won)
            {
                e.PlayerACard.Owner = CardOwner.PlayerB;
                this.playerB.Deck.Add(e.PlayerACard);
                this.playerA.RemoveFromDeck(e.PlayerACard);
                this.playerB.PlayerWinningSteak++;
                this.playerB.Elo += 3;
                this.playerA.Elo -= 5;
            }

            e.PlayerACard.StopPlaying();
            e.PlayerBCard.StopPlaying();
            this.isRoundFinished = true;
            Console.Clear();
        }

        private Tuple<CardOwner, BattleRoundStatus> ProcessSpellVsSpellRound(Card playerACard, Card playerBCard)
        {
            this.battleRound = new SpellVsSpellBattleRound();
            return this.battleRound.Process(playerACard, playerBCard);
        }

        private Tuple<CardOwner, BattleRoundStatus> ProcessMonsterVsSpellRound(Card playerACard, Card playerBCard)
        {
            this.battleRound = new MonsterVsSpellBattleRound();
            return this.battleRound.Process(playerACard, playerBCard);
        }

        private Tuple<CardOwner, BattleRoundStatus> ProcessMonsterVsMonsterRound(Card playerACard, Card playerBCard)
        {
            this.battleRound = new MonsterVsMonsterBattleRound();
            return this.battleRound.Process(playerACard, playerBCard);
        }

        private void PrintCardName(int x, int y, string name)
        {
            Console.SetCursorPosition(x, y);
            Console.WriteLine(name);
        }

        private void OnPositionChanged(object sender, CardPositionChangedEventArgs e)
        {
            lock (locker)
            {
                Console.SetCursorPosition((Console.WindowWidth / 2) - 13, 1);
                Console.WriteLine($"Total Rounds left : {totalRound}");
                Console.SetCursorPosition((Console.WindowWidth / 2) - 8, 2);
                Console.WriteLine($"Current Round: {currentRound}");
                Console.SetCursorPosition(e.OnScreenUpdateArgs.OldX, e.OnScreenUpdateArgs.OldY);
                Console.WriteLine(" ");

                switch (e.OnScreenUpdateArgs.Status)
                {
                    case CardOwner.PlayerA:
                        this.PrintCardName(2, 2, "Player A is playing with " + e.OnScreenUpdateArgs.CardName);
                        Console.SetCursorPosition(2, 3);
                        Console.WriteLine($"Won : {this.playerA.PlayerWinningSteak} Rounds");
                        Console.SetCursorPosition(e.OnScreenUpdateArgs.NewX, e.OnScreenUpdateArgs.NewY);
                        Console.WriteLine(e.OnScreenUpdateArgs.CardName[0]);
                        break;
                    case CardOwner.PlayerB:
                        this.PrintCardName(Console.WindowWidth - 37, 2, "Player B is playing with " + e.OnScreenUpdateArgs.CardName);
                        Console.SetCursorPosition(Console.WindowWidth - 37, 3);
                        Console.WriteLine($"Won : {this.playerB.PlayerWinningSteak} Rounds");
                        Console.SetCursorPosition(e.OnScreenUpdateArgs.NewX, e.OnScreenUpdateArgs.NewY);
                        Console.WriteLine(e.OnScreenUpdateArgs.CardName[0]);
                        break;
                    default:
                        break;

                }
                Console.ResetColor();
            }
        }
    }
}
