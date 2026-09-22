using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RunRich.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _money;
        [SerializeField] private Image _cornerIcon;
        [SerializeField] private Sprite _menuIcon;
        [SerializeField] private Sprite _runIcon;

        public void Show(int money)
        {
            _money.text = money.ToString();
        }

        public void SetRunning(bool running)
        {
            _money.gameObject.SetActive(running);
            _cornerIcon.sprite = running ? _runIcon : _menuIcon;
        }
    }
}
