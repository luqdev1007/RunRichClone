using RunRich.Configs;
using RunRich.Input;
using RunRich.Player;
using RunRich.UI;
using UnityEngine;

namespace RunRich.Core
{
    public sealed class GameBootstrap : MonoBehaviour
    {
        [SerializeField] private PlayerMover _playerMover;
        [SerializeField] private PlayerView _playerView;
        [SerializeField] private PlayerCharacter _playerCharacter;
        [SerializeField] private StatusGaugeView _statusGaugeView;
        [SerializeField] private WealthTierConfig _tierConfig;
        [SerializeField] private Camera _camera;

        private StatusGaugePresenter _statusGaugePresenter;

        private void Awake()
        {
            var wallet = new PlayerWallet();

            _playerMover.Construct(new PointerSwipeInput());
            _playerView.Construct(wallet);
            _playerCharacter.Construct(wallet);

            _statusGaugeView.Construct(_camera);
            _statusGaugePresenter = new StatusGaugePresenter(wallet, _tierConfig, _statusGaugeView);
        }

        private void OnDestroy()
        {
            _statusGaugePresenter?.Dispose();
        }
    }
}
