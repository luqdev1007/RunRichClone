using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RunRich.UI
{
    public sealed class WinScreenView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _reward;
        [SerializeField] private Button _collect;

        public event Action Collected;

        public void Show(int reward)
        {
            _reward.text = reward.ToString();
            gameObject.SetActive(true);
        }

        public void Hide()
        {
            gameObject.SetActive(false);
        }

        private void OnEnable()
        {
            _collect.onClick.AddListener(RaiseCollected);
        }

        private void OnDisable()
        {
            _collect.onClick.RemoveListener(RaiseCollected);
        }

        private void RaiseCollected()
        {
            Collected?.Invoke();
        }
    }
}
