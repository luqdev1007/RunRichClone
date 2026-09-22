using System;
using RunRich.Configs;
using RunRich.Core;
using RunRich.Player;

namespace RunRich.UI
{
    public sealed class StatusGaugePresenter : IDisposable
    {
        private readonly PlayerWallet _wallet;
        private readonly WealthTierConfig _tiers;
        private readonly GameStateMachine _machine;
        private readonly StatusGaugeView _view;

        public StatusGaugePresenter(PlayerWallet wallet, WealthTierConfig tiers,
            GameStateMachine machine, StatusGaugeView view)
        {
            _wallet = wallet;
            _tiers = tiers;
            _machine = machine;
            _view = view;

            _wallet.Changed += OnMoneyChanged;
            _machine.Changed += OnStateChanged;

            Refresh(_wallet.Money);
            _view.SetVisible(_machine.Current == GameState.Playing);
        }

        public void Dispose()
        {
            _wallet.Changed -= OnMoneyChanged;
            _machine.Changed -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            _view.SetVisible(state == GameState.Playing);
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
