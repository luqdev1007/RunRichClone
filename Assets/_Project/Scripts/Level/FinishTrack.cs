using RunRich.Core;
using RunRich.Player;
using UnityEngine;

namespace RunRich.Level
{
    public sealed class FinishTrack : MonoBehaviour
    {
        private const int DefaultMultiplier = 1;

        [SerializeField] private FinishDoor[] _doors;

        private PlayerWallet _wallet;
        private GameLoopController _loop;
        private int _multiplier = DefaultMultiplier;

        public void Construct(PlayerWallet wallet, GameLoopController loop)
        {
            _wallet = wallet;
            _loop = loop;

            foreach (var door in _doors)
                door.Reached += OnDoorReached;
        }

        private void OnDestroy()
        {
            if (_doors == null)
                return;

            foreach (var door in _doors)
            {
                if (door != null)
                    door.Reached -= OnDoorReached;
            }
        }

        private void OnDoorReached(FinishDoor door)
        {
            if (IsBarrier(door))
                FinishRun();
            else
                _multiplier = door.Multiplier;
        }

        private bool IsBarrier(FinishDoor door)
        {
            return _doors.Length > 0 && door == _doors[_doors.Length - 1];
        }

        private void FinishRun()
        {
            _loop.Finish(new FinishResult(_wallet.Money, _multiplier));
            _loop.Win();
        }
    }
}
