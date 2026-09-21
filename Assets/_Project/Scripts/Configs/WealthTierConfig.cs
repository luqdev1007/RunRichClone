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
            return Evaluate(money).Tier;
        }

        public WealthStatus Evaluate(int money)
        {
            var current = FindCurrentTier(money);
            if (current == null)
                return new WealthStatus(null, 0f);

            return new WealthStatus(current, CalculateProgress(current, money));
        }

        private WealthTier FindCurrentTier(int money)
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

        private float CalculateProgress(WealthTier current, int money)
        {
            if (!TryFindNextThreshold(current.MoneyThreshold, out int nextThreshold))
                return 1f;

            float span = nextThreshold - current.MoneyThreshold;
            if (span <= 0f)
                return 1f;

            return Mathf.Clamp01((money - current.MoneyThreshold) / span);
        }

        private bool TryFindNextThreshold(int currentThreshold, out int nextThreshold)
        {
            bool found = false;
            nextThreshold = 0;

            foreach (var tier in _tiers)
            {
                if (tier == null || tier.MoneyThreshold <= currentThreshold)
                    continue;

                if (!found || tier.MoneyThreshold < nextThreshold)
                {
                    nextThreshold = tier.MoneyThreshold;
                    found = true;
                }
            }

            return found;
        }
    }
}
