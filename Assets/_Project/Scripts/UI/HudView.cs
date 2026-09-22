using TMPro;
using UnityEngine;

namespace RunRich.UI
{
    public sealed class HudView : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _money;

        public void Show(int money)
        {
            _money.text = money.ToString();
        }
    }
}
