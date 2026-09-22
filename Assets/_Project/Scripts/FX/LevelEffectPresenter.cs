using System;
using RunRich.Core;
using RunRich.Player;
using UnityEngine;

namespace RunRich.FX
{
    public sealed class LevelEffectPresenter : IDisposable
    {
        private readonly PlayerWallet _wallet;
        private readonly GameStateMachine _machine;
        private readonly PlayerView _view;
        private readonly Transform _anchor;
        private readonly EffectService _effects;

        public LevelEffectPresenter(PlayerWallet wallet, GameStateMachine machine, PlayerView view,
            Transform anchor, EffectService effects)
        {
            _wallet = wallet;
            _machine = machine;
            _view = view;
            _anchor = anchor;
            _effects = effects;

            _wallet.Changed += OnMoneyChanged;
            _machine.Changed += OnStateChanged;
            _view.TierChanged += OnTierChanged;
        }

        public void Dispose()
        {
            _wallet.Changed -= OnMoneyChanged;
            _machine.Changed -= OnStateChanged;
            _view.TierChanged -= OnTierChanged;
        }

        private void OnMoneyChanged(MoneyChange change)
        {
            PlayAtAnchor(change.Delta > 0 ? EffectKind.MoneyBurst : EffectKind.MoneyLoss);
        }

        private void OnTierChanged()
        {
            PlayAtAnchor(EffectKind.TierUp);
        }

        private void OnStateChanged(GameState state)
        {
            if (state == GameState.Win)
                PlayAtAnchor(EffectKind.WinConfetti);
        }

        private void PlayAtAnchor(EffectKind kind)
        {
            _effects.Play(kind, _anchor.position, _anchor.rotation);
        }
    }
}
