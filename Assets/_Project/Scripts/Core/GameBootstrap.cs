using RunRich.Input;
using RunRich.Player;
using UnityEngine;

namespace RunRich.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private PlayerView _playerView;

        public PlayerWallet Wallet { get; private set; }

        private void Awake()
        {
            Wallet = new PlayerWallet();
            _playerMover.Construct(new PointerSwipeInput());
            _playerView.Construct(Wallet);
        }
    }
}
