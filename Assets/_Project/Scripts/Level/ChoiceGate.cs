using System;
using RunRich.Player;
using UnityEngine;

namespace RunRich.Level
{
    public sealed class ChoiceGate : LevelTrigger
    {
        [SerializeField] private int _reward;

        private bool _used;

        public event Action<ChoiceGate> Used;

        public int Reward => _reward;

        protected override void Trigger(PlayerWallet wallet)
        {
            if (_used)
                return;

            _used = true;
            wallet.AddMoney(_reward);
            Used?.Invoke(this);
        }
    }
}
