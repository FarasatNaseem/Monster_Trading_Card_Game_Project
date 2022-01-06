using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MTCG_Battle
{
    public class CardCollisionDetector
    {
        private Card _playerA;
        private Card _playerB;
        private Thread detectorThread;
        private bool isDetectorRunning;
        public CardCollisionDetector(Card playerA, Card playerB)
        {
            this._playerA = playerA;
            this._playerB = playerB;
            this.isDetectorRunning = false;
        }

        public event EventHandler<OnCollisionDetectedEventArgs> OnCollisionDetected;

        public void Start()
        {
            if (!(this.detectorThread != null && this.detectorThread.IsAlive))
            {
                this.detectorThread = new Thread(this.Detect);
                this.isDetectorRunning = false;
                this.detectorThread.Start();
            }
        }

        private void Stop()
        {
            this.isDetectorRunning = true;
            this.detectorThread.Join();
        }

        protected virtual void FireOnCollisionDetected(Card playerA, Card playerB)
        {
            if (OnCollisionDetected != null)
            {
                this.OnCollisionDetected(this, new OnCollisionDetectedEventArgs(playerA, playerB));
            }
        }

        private void Detect()
        {
            while (!this.isDetectorRunning)
            {
                if (_playerA.X == _playerB.X && _playerA.Y == _playerB.Y)
                {
                    this.FireOnCollisionDetected(_playerA, _playerB);
                    break;
                }
            }

            this.Stop();
        }
    }
}
