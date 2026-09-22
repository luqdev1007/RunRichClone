using System;
using RunRich.Player;

namespace RunRich.Core
{
    public sealed class GameLoopController : IDisposable
    {
        private readonly GameStateMachine _machine;
        private readonly PlayerWallet _wallet;
        private readonly PlayerMover _mover;
        private readonly PlayerView _view;

        public GameLoopController(GameStateMachine machine, PlayerWallet wallet, PlayerMover mover, PlayerView view)
        {
            _machine = machine;
            _wallet = wallet;
            _mover = mover;
            _view = view;

            _machine.Changed += OnStateChanged;
            _wallet.Changed += OnMoneyChanged;

            ApplyState(_machine.Current);
        }

        public void StartRun()
        {
            if (_machine.Current != GameState.Menu)
                return;

            _machine.SetState(GameState.Playing);
        }

        public void Finish()
        {
            if (_machine.Current != GameState.Playing)
                return;

            _machine.SetState(GameState.Finish);
        }

        public void Win()
        {
            if (_machine.Current != GameState.Finish)
                return;

            _machine.SetState(GameState.Win);
        }

        public void Dispose()
        {
            _machine.Changed -= OnStateChanged;
            _wallet.Changed -= OnMoneyChanged;
        }

        private void OnMoneyChanged(MoneyChange change)
        {
            if (_machine.Current != GameState.Playing || change.Current > 0)
                return;

            _machine.SetState(GameState.Lose);
        }

        private void OnStateChanged(GameState state)
        {
            ApplyState(state);
        }

        private void ApplyState(GameState state)
        {
            bool running = state == GameState.Playing;
            _mover.SetRunning(running);
            _view.SetMoving(running);
        }
    }
}
