namespace RunRich.Configs
{
    public readonly struct WealthStatus
    {
        public readonly WealthTier Tier;
        public readonly float Progress;

        public WealthStatus(WealthTier tier, float progress)
        {
            Tier = tier;
            Progress = progress;
        }
    }
}
