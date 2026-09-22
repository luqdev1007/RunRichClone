using System;
using RunRich.Player;
using UnityEngine;

namespace RunRich.Level
{
    public sealed class FinishDoor : LevelTrigger
    {
        [SerializeField] private int _multiplier = 2;

        private bool _reached;

        public event Action<FinishDoor> Reached;

        public int Multiplier => _multiplier;

        protected override void Trigger(PlayerWallet wallet)
        {
            if (_reached)
                return;

            _reached = true;
            Reached?.Invoke(this);
        }
    }
}
