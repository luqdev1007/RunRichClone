using System.Collections.Generic;
using UnityEngine;

namespace RunRich.Configs
{
    [CreateAssetMenu(menuName = "RunRich/Wealth Tier Config", fileName = "WealthTierConfig")]
    public sealed class WealthTierConfig : ScriptableObject
    {
        [SerializeField] private WealthTier[] _tiers;

        public IReadOnlyList<WealthTier> Tiers => _tiers;

        public WealthTier Resolve(int money)
        {
            WealthTier reached = null;
            WealthTier lowest = null;

            foreach (var tier in _tiers)
            {
                if (tier == null)
                    continue;

                if (lowest == null || tier.MoneyThreshold < lowest.MoneyThreshold)
                    lowest = tier;

                if (tier.MoneyThreshold > money)
                    continue;

                if (reached == null || tier.MoneyThreshold > reached.MoneyThreshold)
                    reached = tier;
            }

            return reached ?? lowest;
        }
    }
}
