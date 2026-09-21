using RunRich.Input;
using RunRich.Player;
using UnityEngine;

namespace RunRich.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private PlayerCharacter _playerCharacter;

        private void Awake()
        {
            var wallet = new PlayerWallet();

            _playerMover.Construct(new PointerSwipeInput());
            _playerView.Construct(wallet);
            _playerCharacter.Construct(wallet);
        }
    }
}
