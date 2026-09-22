using RunRich.Configs;
using UnityEngine;

namespace RunRich.Player
{
    public sealed class PlayerView : MonoBehaviour
    {
        private const string GaitParameter = "Gait";
        private const string ModeParameter = "Mode";
        private const string SpinParameter = "Spin";

        private static readonly int GaitHash = Animator.StringToHash(GaitParameter);
        private static readonly int ModeHash = Animator.StringToHash(ModeParameter);
        private static readonly int SpinHash = Animator.StringToHash(SpinParameter);

        [SerializeField] private Animator _animator;
        [SerializeField] private WealthTierConfig _tierConfig;

        private SkinnedMeshRenderer[] _outfits;
        private PlayerWallet _wallet;
        private WealthTier _currentTier;

        public int CurrentGaitId => _currentTier == null ? 0 : _currentTier.GaitId;

        private SkinnedMeshRenderer[] Outfits =>
            _outfits ??= GetComponentsInChildren<SkinnedMeshRenderer>(true);

        public void Construct(PlayerWallet wallet)
        {
            _wallet = wallet;
            _wallet.Changed += OnMoneyChanged;
            ApplyTier(_wallet.Money);
        }

        public void SetMode(PlayerAnimationMode mode)
        {
            _animator.SetInteger(ModeHash, (int)mode);
        }

        private void OnDestroy()
        {
            if (_wallet != null)
                _wallet.Changed -= OnMoneyChanged;
        }

        private void OnMoneyChanged(MoneyChange change)
        {
            ApplyTier(change.Current);
        }

        private void ApplyTier(int money)
        {
            var tier = _tierConfig.Resolve(money);
            if (tier == null || tier == _currentTier)
                return;

            bool isChange = _currentTier != null;
            _currentTier = tier;

            ShowOutfit(tier.OutfitMesh);
            _animator.SetInteger(GaitHash, tier.GaitId);

            if (isChange)
                _animator.SetTrigger(SpinHash);
        }

        private void ShowOutfit(Mesh outfitMesh)
        {
            foreach (var outfit in Outfits)
                outfit.enabled = outfit.sharedMesh == outfitMesh;
        }
    }
}
