using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTCG_Battle
{
    public abstract class Card
    {
        private int movingStep = 1;
        private int _x;
        private int _y;
        private CardThreadArgs threadArgs;
        private Thread cardThread;
        public Card() { }

        public Card(string name, double damage, CardElementType type, CardOwner cardStatus)
        {
            try
            {
                this.Name = name;
                this.Damage = damage;
                this.ElementType = type;
                this.Owner = cardStatus;
                this.X = this.Owner == CardOwner.PlayerA ? 0 : Console.WindowWidth - 1;
                this.Y = 13;/* Console.WindowHeight % 2 == 0 ? Console.WindowHeight / 2 : (Console.WindowHeight + 1) / 2;*/
                this.Color = this.Owner == CardOwner.PlayerA ? CardColor.Yellow : CardColor.Blue;
                this.Direction = this.Owner == CardOwner.PlayerA ? Direction.Right : Direction.Left;
                this.threadArgs = new CardThreadArgs();
            }
            catch (Exception)
            {
            }
        }

        public Card(string name) 
        {
            this.Name = name; 
        }
       

        public Card(int x, int y)
        {
            this.X = x;
            this.Y = y;
        }

        public event EventHandler<CardPositionChangedEventArgs> OnPositionChanged;

        public string Name
        {
            get;
            private set;
        }

        public double Damage
        {
            get;
            set;
        }

        public CardElementType ElementType
        {
            get;
            private set;
        }

        public Direction Direction
        {
            get;
            set;
        }

        public CardColor Color
        {
            get;
            set;
        }

        public int X
        {
            get
            {
                return this._x;
            }
            set
            {
                if (this._x == value)
                {
                    return;
                }

                this.FireOnPositionChanged(new OnScreenUpdateArgs(this._x, this._y, value, this._y, this.Name, this.Owner));

                this._x = value;
            }
        }

        public int Y
        {
            get
            {
                return this._y;
            }
            set
            {
                if (this._y == value)
                {
                    return;
                }

                this.FireOnPositionChanged(new OnScreenUpdateArgs(this._x, this._y, this._x, value, this.Name, this.Owner));

                this._y = value;
            }
        }

        public CardOwner Owner
        {
            get;
            set;
        }

        public void StartPlaying()
        {
            if (!(this.cardThread != null && this.cardThread.IsAlive))
            {
                this.cardThread = new Thread(this.Move);
                this.threadArgs.Exit = false;
                this.cardThread.Start(this.threadArgs);
            }
        }

        public void StopPlaying()
        {
            if (this.cardThread != null)
            {
                this.threadArgs.Exit = true;
                this.cardThread.Join();
            }
        }

        private void Move(object data)
        {
            var args = (CardThreadArgs)data;

            while (!args.Exit)
            {
                Thread.Sleep(10);
                switch (this.Direction)
                {
                    case Direction.Right:
                        this.X += this.movingStep;
                        break;
                    case Direction.Left:
                        this.X -= this.movingStep;
                        break;
                    case Direction.Up:
                        this.Y -= this.movingStep;
                        break;
                    case Direction.Down:
                        this.Y += this.movingStep;
                        break;
                    case Direction.None:
                        break;
                    default:
                        break;
                }

                try
                {
                    if (this.X == Console.WindowWidth - 1)
                        this.Direction = Direction.Left;
                    if (this.X == 0)
                        this.Direction = Direction.Right;
                }
                catch (Exception)
                {
                }
            }
        }

        protected virtual void FireOnPositionChanged(OnScreenUpdateArgs onScreenUpdateArgs)
        {
            if (this.OnPositionChanged == null)
                return;
            this.OnPositionChanged(this, new CardPositionChangedEventArgs(onScreenUpdateArgs));
        }
    }
}
