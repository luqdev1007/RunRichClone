namespace RunRich.Core
{
    public readonly struct FinishResult
    {
        public readonly int Money;
        public readonly int Multiplier;

        public FinishResult(int money, int multiplier)
        {
            Money = money;
            Multiplier = multiplier;
        }

        public int Reward => Money * Multiplier;
    }
}
