using System;
using DG.Tweening;
using RunRich.Input;
using UnityEngine;

namespace RunRich.UI
{
    public sealed class StartScreenView : MonoBehaviour
    {
        [SerializeField] private RectTransform _hand;
        [SerializeField] private float _handTravel = 120f;
        [SerializeField] private float _handDuration = 0.9f;

        private IInputService _input;
        private Vector2 _handOrigin;
        private Tween _handTween;

        public event Action StartRequested;

        public void Construct(IInputService input)
        {
            _input = input;
        }

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void Awake()
        {
            _handOrigin = _hand.anchoredPosition;
        }

        private void OnEnable()
        {
            _hand.anchoredPosition = _handOrigin;
            _handTween = _hand.DOAnchorPosX(_handOrigin.x + _handTravel, _handDuration)
                .SetEase(Ease.InOutSine)
                .SetLoops(-1, LoopType.Yoyo)
                .SetLink(gameObject);
        }

        private void OnDisable()
        {
            _handTween?.Kill();
            _handTween = null;
            _hand.anchoredPosition = _handOrigin;
        }

        private void Update()
        {
            if (_input != null && _input.PressedThisFrame)
                StartRequested?.Invoke();
        }
    }
}
