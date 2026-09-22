using System;
using RunRich.Player;

namespace RunRich.UI
{
    public sealed class HudPresenter : IDisposable
    {
        private readonly PlayerWallet _wallet;
        private readonly HudView _view;

        public HudPresenter(PlayerWallet wallet, HudView view)
        {
            _wallet = wallet;
            _view = view;

            _wallet.Changed += OnMoneyChanged;
            _view.Show(_wallet.Money);
        }

        public void Dispose()
        {
            _wallet.Changed -= OnMoneyChanged;
        }

        private void OnMoneyChanged(MoneyChange change)
        {
            _view.Show(change.Current);
        }
    }
}
