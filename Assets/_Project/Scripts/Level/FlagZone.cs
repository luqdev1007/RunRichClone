using DG.Tweening;
using RunRich.Player;
using UnityEngine;

namespace RunRich.Level
{
    public sealed class FlagZone : LevelTrigger
    {
        [SerializeField] private Transform[] _flags;
        [SerializeField] private float _lyingAngle = -85f;
        [SerializeField] private float _raiseDuration = 0.45f;
        [SerializeField] private Ease _raiseEase = Ease.OutBack;
        [SerializeField] private float _hopHeight = 0.15f;
        [SerializeField] private float _hopDuration = 0.25f;
        [SerializeField] private float _flagDelay = 0.08f;

        private bool _raised;

        private void Awake()
        {
            foreach (var flag in _flags)
                flag.localRotation = Quaternion.Euler(_lyingAngle, 0f, 0f);
        }

        protected override void Trigger(PlayerWallet wallet)
        {
            if (_raised)
                return;

            _raised = true;
            RaiseFlags();
        }

        private void RaiseFlags()
        {
            for (int i = 0; i < _flags.Length; i++)
                RaiseFlag(_flags[i], i * _flagDelay);
        }

        private void RaiseFlag(Transform flag, float delay)
        {
            flag.DOLocalRotate(Vector3.zero, _raiseDuration)
                .SetEase(_raiseEase)
                .SetDelay(delay)
                .SetLink(gameObject);

            flag.DOLocalMoveY(flag.localPosition.y + _hopHeight, _hopDuration)
                .SetEase(Ease.OutQuad)
                .SetDelay(delay)
                .SetLoops(2, LoopType.Yoyo)
                .SetLink(gameObject);
        }
    }
}
