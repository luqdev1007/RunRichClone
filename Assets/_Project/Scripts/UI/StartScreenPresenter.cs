using System;
using RunRich.Core;

namespace RunRich.UI
{
    public sealed class StartScreenPresenter : IDisposable
    {
        private readonly GameStateMachine _machine;
        private readonly GameLoopController _loop;
        private readonly StartScreenView _view;

        public StartScreenPresenter(GameStateMachine machine, GameLoopController loop, StartScreenView view)
        {
            _machine = machine;
            _loop = loop;
            _view = view;

            Apply(_machine.Current);
            _machine.Changed += OnStateChanged;
            _view.StartRequested += OnStartRequested;
        }

        public void Dispose()
        {
            _machine.Changed -= OnStateChanged;
            _view.StartRequested -= OnStartRequested;
        }

        private void OnStateChanged(GameState state)
        {
            Apply(state);
        }

        private void Apply(GameState state)
        {
            if (state == GameState.Menu)
                _view.Show();
            else
                _view.Hide();
        }

        private void OnStartRequested()
        {
            _loop.StartRun();
        }
    }
}
