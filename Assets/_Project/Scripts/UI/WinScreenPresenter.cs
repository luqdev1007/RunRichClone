using System;
using ButchersGames;
using RunRich.Core;

namespace RunRich.UI
{
    public sealed class WinScreenPresenter : IDisposable
    {
        private readonly GameStateMachine _machine;
        private readonly GameLoopController _loop;
        private readonly WinScreenView _view;

        public WinScreenPresenter(GameStateMachine machine, GameLoopController loop, WinScreenView view)
        {
            _machine = machine;
            _loop = loop;
            _view = view;

            _view.Hide();
            _machine.Changed += OnStateChanged;
            _view.Collected += OnCollected;
        }

        public void Dispose()
        {
            _machine.Changed -= OnStateChanged;
            _view.Collected -= OnCollected;
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Win)
                _view.Show(_loop.Result.Reward);
            else
                _view.Hide();
        }

        private void OnCollected()
        {
            LevelManager.Default.NextLevel();
        }
    }
}
