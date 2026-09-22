using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RunRich.UI
{
    public sealed class StatusGaugeView : MonoBehaviour
    {
        [SerializeField] private Transform _billboard;
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private Image _fill;

        private Camera _camera;

        public void Construct(Camera camera)
        {
            _camera = camera;
        }

        public void SetVisible(bool visible)
        {
            _billboard.gameObject.SetActive(visible);
        }

        public void Show(string title, Color color, float progress)
        {
            _title.text = title;
            _title.color = color;
            _fill.color = color;
            _fill.fillAmount = progress;
        }

        private void LateUpdate()
        {
            if (_camera == null)
                return;

            _billboard.rotation = _camera.transform.rotation;
        }
    }
}
