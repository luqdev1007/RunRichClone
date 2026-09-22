using RunRich.Configs;
using RunRich.FX;
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
        [SerializeField] private MoneyPopupView _moneyPopupView;
        [SerializeField] private WealthTierConfig _tierConfig;
        [SerializeField] private MoneyPopupConfig _moneyPopupConfig;
        [SerializeField] private Camera _camera;

        private StatusGaugePresenter _statusGaugePresenter;
        private MoneyPopupAccumulator _moneyPopupAccumulator;
        private MoneyPopupPresenter _moneyPopupPresenter;

        private void Awake()
        {
            var wallet = new PlayerWallet();

            _playerMover.Construct(new PointerSwipeInput());
            _playerView.Construct(wallet);
            _playerCharacter.Construct(wallet);

            _statusGaugeView.Construct(_camera);
            _statusGaugePresenter = new StatusGaugePresenter(wallet, _tierConfig, _statusGaugeView);

            _moneyPopupView.Construct(_camera, _moneyPopupConfig);
            _moneyPopupAccumulator = new MoneyPopupAccumulator(wallet, _moneyPopupConfig.IdlePause);
            _moneyPopupPresenter = new MoneyPopupPresenter(_moneyPopupAccumulator, _moneyPopupConfig, _moneyPopupView);
        }

        private void OnDestroy()
        {
            _statusGaugePresenter?.Dispose();
            _moneyPopupPresenter?.Dispose();
            _moneyPopupAccumulator?.Dispose();
        }
    }
}
