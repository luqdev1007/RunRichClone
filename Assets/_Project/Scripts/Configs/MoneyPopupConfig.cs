using DG.Tweening;
using UnityEngine;

namespace RunRich.Configs
{
    [CreateAssetMenu(menuName = "RunRich/Money Popup Config", fileName = "MoneyPopupConfig")]
    public sealed class MoneyPopupConfig : ScriptableObject
    {
        [SerializeField] private float _idlePause = 0.3f;
        [SerializeField] private float _fadeDuration = 0.3f;
        [SerializeField] private Vector2 _fadeOffset = new Vector2(-25f, 45f);
        [SerializeField] private Ease _fadeEase = Ease.OutQuad;
        [SerializeField] private Color _positiveColor = new Color(0.2f, 0.85f, 0.25f);
        [SerializeField] private Color _negativeColor = new Color(0.9f, 0.15f, 0.15f);

        public float IdlePause => _idlePause;
        public float FadeDuration => _fadeDuration;
        public Vector2 FadeOffset => _fadeOffset;
        public Ease FadeEase => _fadeEase;
        public Color PositiveColor => _positiveColor;
        public Color NegativeColor => _negativeColor;
    }
}
