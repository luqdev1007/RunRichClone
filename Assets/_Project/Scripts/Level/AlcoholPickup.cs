using UnityEngine;

namespace RunRich.Level
{
    public sealed class AlcoholPickup : Pickup
    {
        [SerializeField] private int _penalty = 20;

        protected override int MoneyDelta => -_penalty;
    }
}
