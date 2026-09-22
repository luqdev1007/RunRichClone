using System;
using UnityEngine;
using UnityEngine.UI;

namespace RunRich.UI
{
    public sealed class LoseScreenView : MonoBehaviour
    {
        [SerializeField] private Button _retry;

        public event Action RetryRequested;

        public void Show()
        {
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _retry.onClick.AddListener(RaiseRetry);
        }

        private void OnDisable()
        {
            _retry.onClick.RemoveListener(RaiseRetry);
        }

        private void RaiseRetry()
        {
            RetryRequested?.Invoke();
        }
    }
}
