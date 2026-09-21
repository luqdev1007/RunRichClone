using RunRich.Player;

namespace RunRich.Level
{
    public abstract class Pickup : LevelTrigger
    {
        private bool _collected;

        protected abstract int MoneyDelta { get; }

        protected sealed override void Trigger(PlayerWallet wallet)
        {
            if (_collected)
                return;

            _collected = true;
            wallet.AddMoney(MoneyDelta);
            gameObject.SetActive(false);
        }
    }
}
