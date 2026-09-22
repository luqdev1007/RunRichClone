using System;
using RunRich.Core;
using RunRich.Level;
using RunRich.Player;
using UnityEngine;

namespace RunRich.FX
{
    public sealed class LevelEffectPresenter : IDisposable
    {
        private const float DoorEffectHeight = 1.4f;

        private static readonly Color[] DoorColors =
        {
            new Color(1f, 0.55f, 0.10f),
            new Color(1f, 0.55f, 0.10f),
            new Color(0.22f, 0.72f, 0.30f),
            new Color(0.30f, 0.45f, 1f)
        };

        private readonly PlayerWallet _wallet;
        private readonly GameStateMachine _machine;
        private readonly PlayerView _view;
        private readonly Transform _anchor;
        private readonly FinishDoor[] _doors;
        private readonly EffectService _effects;

        public LevelEffectPresenter(PlayerWallet wallet, GameStateMachine machine, PlayerView view,
            Transform anchor, FinishDoor[] doors, EffectService effects)
        {
            _wallet = wallet;
            _machine = machine;
            _view = view;
            _anchor = anchor;
            _doors = doors;
            _effects = effects;

            _wallet.Changed += OnMoneyChanged;
            _machine.Changed += OnStateChanged;
            _view.TierChanged += OnTierChanged;

            foreach (var door in _doors)
                door.Reached += OnDoorReached;
        }

        public void Dispose()
        {
            _wallet.Changed -= OnMoneyChanged;
            _machine.Changed -= OnStateChanged;
            _view.TierChanged -= OnTierChanged;

            foreach (var door in _doors)
            {
                if (door != null)
                    door.Reached -= OnDoorReached;
            }
        }

        private void OnDoorReached(FinishDoor door)
        {
            int index = Array.IndexOf(_doors, door);
            var color = index >= 0 && index < DoorColors.Length ? DoorColors[index] : Color.white;

            _effects.Play(EffectKind.DoorPass,
                door.transform.position + Vector3.up * DoorEffectHeight,
                door.transform.rotation, color);
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
