using UnityEngine;

namespace RunRich.Level
{
    public sealed class MoneyPickup : Pickup
    {
        [SerializeField] private int _reward = 4;

        protected override int MoneyDelta => _reward;
    }
}
