namespace MTCG_Battle
{
    public class CardPositionChangedEventArgs
    {
        public CardPositionChangedEventArgs(OnScreenUpdateArgs onScreenUpdateArgs)
        {
            this.OnScreenUpdateArgs = onScreenUpdateArgs;
        }

        public OnScreenUpdateArgs OnScreenUpdateArgs
        {
            get;
            private set;
        }
    }
}