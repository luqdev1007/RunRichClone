using System;
using RunRich.Core;

namespace RunRich.UI
{
    public sealed class WinScreenPresenter : IDisposable
    {
        private readonly GameStateMachine _machine;
        private readonly GameLoopController _loop;
        private readonly LevelFlow _levelFlow;
        private readonly WinScreenView _view;

        public WinScreenPresenter(GameStateMachine machine, GameLoopController loop,
            LevelFlow levelFlow, WinScreenView view)
        {
            _machine = machine;
            _loop = loop;
            _levelFlow = levelFlow;
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
            _levelFlow.Next();
        }
    }
}
