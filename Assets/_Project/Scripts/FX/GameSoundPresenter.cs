using System;
using RunRich.Configs;
using RunRich.Core;
using RunRich.Player;

namespace RunRich.FX
{
    public sealed class GameSoundPresenter : IDisposable
    {
        private readonly PlayerWallet _wallet;
        private readonly GameStateMachine _machine;
        private readonly AudioService _audio;
        private readonly SoundConfig _config;

        public GameSoundPresenter(PlayerWallet wallet, GameStateMachine machine,
            AudioService audio, SoundConfig config)
        {
            _wallet = wallet;
            _machine = machine;
            _audio = audio;
            _config = config;

            _wallet.Changed += OnMoneyChanged;
            _machine.Changed += OnStateChanged;
        }

        public void Dispose()
        {
            _wallet.Changed -= OnMoneyChanged;
            _machine.Changed -= OnStateChanged;
        }

        private void OnMoneyChanged(MoneyChange change)
        {
            if (change.Delta > 0)
                _audio.PlayRandom(_config.MoneyPickup, _config.EffectVolume);
            else
                _audio.Play(_config.MoneyLoss, _config.EffectVolume);
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Win)
                _audio.Play(_config.Win, _config.EffectVolume);
            else if (state == GameState.Lose)
                _audio.Play(_config.Lose, _config.EffectVolume);
        }
    }
}
