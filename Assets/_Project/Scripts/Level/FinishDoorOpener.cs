using DG.Tweening;
using RunRich.Player;
using UnityEngine;

namespace RunRich.Level
{
    public sealed class FinishDoorOpener : LevelTrigger
    {
        [SerializeField] private Transform[] _hinges;
        [SerializeField] private float _openAngle = 95f;
        [SerializeField] private float _duration = 0.45f;
        [SerializeField] private Ease _ease = Ease.OutBack;

        private bool _opened;

        protected override void Trigger(PlayerWallet wallet)
        {
            if (_opened)
                return;

            _opened = true;
            Open();
        }

        private void Open()
        {
            foreach (var hinge in _hinges)
            {
                float side = Mathf.Sign(hinge.localPosition.x);
                hinge.DOLocalRotate(new Vector3(0f, _openAngle * side, 0f), _duration)
                    .SetEase(_ease)
                    .SetLink(gameObject);
            }
        }
    }
}
