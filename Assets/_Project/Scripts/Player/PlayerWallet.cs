using System;

namespace RunRich.Player
{
    public sealed class PlayerWallet
    {
        public int Money { get; private set; }

        public event Action<MoneyChange> Changed;

        public void AddMoney(int delta)
        {
            if (delta == 0)
                return;

            int previous = Money;
            Money = previous + delta;
            Changed?.Invoke(new MoneyChange(previous, Money));
        }
    }
}
