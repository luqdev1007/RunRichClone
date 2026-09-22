using System;
using RunRich.Core;

namespace RunRich.UI
{
    public sealed class LoseScreenPresenter : IDisposable
    {
        private readonly GameStateMachine _machine;
        private readonly LevelFlow _levelFlow;
        private readonly LoseScreenView _view;

        public LoseScreenPresenter(GameStateMachine machine, LevelFlow levelFlow, LoseScreenView view)
        {
            _machine = machine;
            _levelFlow = levelFlow;
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
            _levelFlow.Restart();
        }
    }
}
