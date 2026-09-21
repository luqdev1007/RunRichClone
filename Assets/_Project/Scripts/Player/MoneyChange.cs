namespace RunRich.Player
{
    public readonly struct MoneyChange
    {
        public readonly int Previous;
        public readonly int Current;
        public readonly int Delta;

        public MoneyChange(int previous, int current)
        {
            Previous = previous;
            Current = current;
            Delta = current - previous;
        }
    }
}
