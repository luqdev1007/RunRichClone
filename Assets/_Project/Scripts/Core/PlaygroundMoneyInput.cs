#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

namespace RunRich.Core
{
    public sealed class PlaygroundMoneyInput : MonoBehaviour
    {
        [SerializeField] private GameBootstrap _bootstrap;
        [SerializeField] private int _smallReward = 10;
        [SerializeField] private int _largeReward = 50;
        [SerializeField] private int _penalty = -20;

        private void Update()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null || _bootstrap.Wallet == null)
                return;

            if (keyboard.digit1Key.wasPressedThisFrame)
                _bootstrap.Wallet.AddMoney(_smallReward);

            if (keyboard.digit2Key.wasPressedThisFrame)
                _bootstrap.Wallet.AddMoney(_largeReward);

            if (keyboard.digit3Key.wasPressedThisFrame)
                _bootstrap.Wallet.AddMoney(_penalty);
        }
    }
}
#endif
