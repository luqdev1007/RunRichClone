using System;
using ButchersGames;
using RunRich.Core;

namespace RunRich.UI
{
    public sealed class LoseScreenPresenter : IDisposable
    {
        private readonly GameStateMachine _machine;
        private readonly LoseScreenView _view;

        public LoseScreenPresenter(GameStateMachine machine, LoseScreenView view)
        {
            _machine = machine;
            _view = view;

            _view.Hide();
            _machine.Changed += OnStateChanged;
            _view.RetryRequested += OnRetryRequested;
        }

        public void Dispose()
        {
            _machine.Changed -= OnStateChanged;
            _view.RetryRequested -= OnRetryRequested;
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Lose)
                _view.Show();
            else
                _view.Hide();
        }

        private void OnRetryRequested()
        {
            LevelManager.Default.RestartLevel();
        }
    }
}
