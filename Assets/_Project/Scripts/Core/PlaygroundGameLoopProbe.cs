using RunRich.Player;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RunRich.Core
{
    public sealed class PlaygroundGameLoopProbe : MonoBehaviour
    {
        private const int LosePenalty = -1000;
        private const int ProbeMultiplier = 1;

        private PlayerWallet _wallet;
        private GameLoopController _loop;
        private GameStateMachine _machine;

        public void Construct(PlayerWallet wallet, GameLoopController loop, GameStateMachine machine)
        {
            _wallet = wallet;
            _loop = loop;
            _machine = machine;

            _machine.Changed += OnStateChanged;
        }

        private void OnDestroy()
        {
            if (_machine != null)
                _machine.Changed -= OnStateChanged;
        }

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || _wallet == null)
                return;

            if (keyboard.lKey.wasPressedThisFrame)
                _wallet.AddMoney(LosePenalty);

            if (keyboard.fKey.wasPressedThisFrame)
                _loop.Finish(new FinishResult(_wallet.Money, ProbeMultiplier));

            if (keyboard.wKey.wasPressedThisFrame)
                _loop.Win();
        }

        private void OnStateChanged(GameState state)
        {
            Debug.Log($"[GameLoop] {state}");
        }
    }
}
