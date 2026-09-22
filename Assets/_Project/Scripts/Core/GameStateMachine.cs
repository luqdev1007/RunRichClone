using System;

namespace RunRich.Core
{
    public sealed class GameStateMachine
    {
        public GameState Current { get; private set; }

        public event Action<GameState> Changed;

        public void SetState(GameState state)
        {
            if (Current == state)
                return;

            Current = state;
            Changed?.Invoke(state);
        }
    }
}
