using RunRich.Input;
using RunRich.Player;
using UnityEngine;

namespace RunRich.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;

        private void Awake()
        {
            _playerMover.Construct(new PointerSwipeInput());
        }
    }
}
