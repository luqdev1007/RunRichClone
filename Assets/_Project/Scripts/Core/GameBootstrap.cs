using ButchersGames;
using RunRich.Configs;
using RunRich.FX;
using RunRich.Input;
using RunRich.Level;
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
        [SerializeField] private HudView _hudView;
        [SerializeField] private WinScreenView _winScreenView;
        [SerializeField] private LoseScreenView _loseScreenView;
        [SerializeField] private StartScreenView _startScreenView;
        [SerializeField] private WealthTierConfig _tierConfig;
        [SerializeField] private MoneyPopupConfig _moneyPopupConfig;
        [SerializeField] private SoundConfig _soundConfig;
        [SerializeField] private AudioService _audioService;
        [SerializeField] private FootstepPlayer _footstepPlayer;
        [SerializeField] private Camera _camera;
        [SerializeField] private int _startingMoney = 40;
        [SerializeField] private LevelManager _levelManager;
        [SerializeField] private PlaygroundGameLoopProbe _playgroundProbe;

        private StatusGaugePresenter _statusGaugePresenter;
        private HudPresenter _hudPresenter;
        private WinScreenPresenter _winScreenPresenter;
        private LoseScreenPresenter _loseScreenPresenter;
        private StartScreenPresenter _startScreenPresenter;
        private GameSoundPresenter _gameSoundPresenter;
        private LevelSoundPresenter _levelSoundPresenter;
        private MoneyPopupAccumulator _moneyPopupAccumulator;
        private MoneyPopupPresenter _moneyPopupPresenter;
        private GameStateMachine _gameState;
        private GameLoopController _gameLoop;

        private void Awake()
        {
            _levelManager.Init();
            var level = _levelManager.GetComponentInChildren<LevelSetup>();

            var wallet = new PlayerWallet(_startingMoney);
            var input = new PointerSwipeInput();

            _playerCharacter.transform.SetPositionAndRotation(
                level.PlayerSpawn.position, level.PlayerSpawn.rotation);

            _playerMover.Construct(input, level.Road);
            _startScreenView.Construct(input);
            _playerView.Construct(wallet);
            _playerCharacter.Construct(wallet);

            _statusGaugeView.Construct(_camera);

            _moneyPopupView.Construct(_camera, _moneyPopupConfig);
            _moneyPopupAccumulator = new MoneyPopupAccumulator(wallet, _moneyPopupConfig.IdlePause);
            _moneyPopupPresenter = new MoneyPopupPresenter(_moneyPopupAccumulator, _moneyPopupConfig, _moneyPopupView);

            var levelFlow = new LevelFlow(_levelManager);

            _gameState = new GameStateMachine();
            _gameLoop = new GameLoopController(_gameState, wallet, _playerMover, _playerView);

            _statusGaugePresenter = new StatusGaugePresenter(wallet, _tierConfig, _gameState, _statusGaugeView);
            _hudPresenter = new HudPresenter(wallet, _gameState, _hudView);
            _winScreenPresenter = new WinScreenPresenter(_gameState, _gameLoop, levelFlow, _winScreenView);
            _loseScreenPresenter = new LoseScreenPresenter(_gameState, levelFlow, _loseScreenView);
            _startScreenPresenter = new StartScreenPresenter(_gameState, _gameLoop, _startScreenView);

            level.FinishTrack.Construct(wallet, _gameLoop);

            _gameSoundPresenter = new GameSoundPresenter(wallet, _gameState, _audioService, _soundConfig);
            _levelSoundPresenter = new LevelSoundPresenter(
                level.Gates, level.FinishTrack.Doors, _audioService, _soundConfig);
            _footstepPlayer.Construct(_playerMover, _playerView, _audioService, _soundConfig);

            if (_playgroundProbe != null)
                _playgroundProbe.Construct(wallet, _gameLoop, _gameState);
        }

        private void OnDestroy()
        {
            _statusGaugePresenter?.Dispose();
            _hudPresenter?.Dispose();
            _winScreenPresenter?.Dispose();
            _loseScreenPresenter?.Dispose();
            _startScreenPresenter?.Dispose();
            _gameSoundPresenter?.Dispose();
            _levelSoundPresenter?.Dispose();
            _moneyPopupPresenter?.Dispose();
            _moneyPopupAccumulator?.Dispose();
            _gameLoop?.Dispose();
        }
    }
}
