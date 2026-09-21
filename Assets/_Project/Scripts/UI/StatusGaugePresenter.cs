using System;
using RunRich.Configs;
using RunRich.Player;

namespace RunRich.UI
{
    public sealed class StatusGaugePresenter : IDisposable
    {
        private readonly PlayerWallet _wallet;
        private readonly WealthTierConfig _tiers;
        private readonly StatusGaugeView _view;

        public StatusGaugePresenter(PlayerWallet wallet, WealthTierConfig tiers, StatusGaugeView view)
        {
            _wallet = wallet;
            _tiers = tiers;
            _view = view;

            _wallet.Changed += OnMoneyChanged;
            Refresh(_wallet.Money);
        }

        public void Dispose()
        {
            _wallet.Changed -= OnMoneyChanged;
        }

        private void OnMoneyChanged(MoneyChange change)
        {
            Refresh(change.Current);
        }

        private void Refresh(int money)
        {
            var status = _tiers.Evaluate(money);
            if (status.Tier == null)
                return;

            _view.Show(status.Tier.Title, status.Tier.GaugeColor, status.Progress);
        }
    }
}
