using DG.Tweening;
using RunRich.Configs;
using TMPro;
using UnityEngine;

namespace RunRich.UI
{
    public sealed class MoneyPopupView : MonoBehaviour
    {
        [SerializeField] private Transform _billboard;
        [SerializeField] private RectTransform _label;
        [SerializeField] private CanvasGroup _group;
        [SerializeField] private TextMeshProUGUI _text;

        private MoneyPopupConfig _config;
        private Camera _camera;
        private Vector2 _origin;
        private Sequence _fade;

        public void Construct(Camera camera, MoneyPopupConfig config)
        {
            _camera = camera;
            _config = config;
            _origin = _label.anchoredPosition;
        }

        public void Show(string amount, Color color)
        {
            KillFade();

            _label.anchoredPosition = _origin;
            _group.alpha = 1f;
            _text.text = amount;
            _text.color = color;
        }

        public void FadeOut()
        {
            KillFade();

            _fade = DOTween.Sequence()
                .Join(_label.DOAnchorPos(_origin + _config.FadeOffset, _config.FadeDuration))
                .Join(_group.DOFade(0f, _config.FadeDuration))
                .SetEase(_config.FadeEase)
                .SetLink(gameObject);
        }

        public void Hide()
        {
            KillFade();
            _group.alpha = 0f;
        }

        private void KillFade()
        {
            _fade?.Kill();
            _fade = null;
        }

        private void LateUpdate()
        {
            if (_camera == null)
                return;

            _billboard.rotation = _camera.transform.rotation;
        }
    }
}
