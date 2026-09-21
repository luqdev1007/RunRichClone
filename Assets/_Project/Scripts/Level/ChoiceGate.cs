using RunRich.Player;
using UnityEngine;

namespace RunRich.Level
{
    public sealed class ChoiceGate : LevelTrigger
    {
        [SerializeField] private int _reward;

        private bool _used;

        protected override void Trigger(PlayerWallet wallet)
        {
            if (_used)
                return;

            _used = true;
            wallet.AddMoney(_reward);
        }
    }
}
