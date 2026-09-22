using System;
using RunRich.Core;
using RunRich.Player;

namespace RunRich.UI
{
    public sealed class HudPresenter : IDisposable
    {
        private readonly PlayerWallet _wallet;
        private readonly GameStateMachine _machine;
        private readonly HudView _view;

        public HudPresenter(PlayerWallet wallet, GameStateMachine machine, HudView view)
        {
            _wallet = wallet;
            _machine = machine;
            _view = view;

            _wallet.Changed += OnMoneyChanged;
            _machine.Changed += OnStateChanged;

            _view.Show(_wallet.Money);
            _view.SetMoneyVisible(_machine.Current == GameState.Playing);
        }

        public void Dispose()
        {
            _wallet.Changed -= OnMoneyChanged;
            _machine.Changed -= OnStateChanged;
        }

        private void OnStateChanged(GameState state)
        {
            _view.SetMoneyVisible(state == GameState.Playing);
        }

        private void OnMoneyChanged(MoneyChange change)
        {
            _view.Show(change.Current);
        }
    }
}
