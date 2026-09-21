using UnityEngine;

namespace RunRich.Configs
{
    [CreateAssetMenu(menuName = "RunRich/Player Movement Config", fileName = "PlayerMovementConfig")]
    public sealed class PlayerMovementConfig : ScriptableObject
    {
        [SerializeField] private float _forwardSpeed = 7f;
        [SerializeField] private float _sensitivity = 9f;
        [SerializeField] private float _roadHalfWidth = 2.4f;
        [SerializeField, Range(0f, 89f)] private float _maxYawAngle = 35f;
        [SerializeField] private float _yawSmoothTime = 0.1f;
        [SerializeField] private float _lateralSmoothTime = 0.06f;

        public float ForwardSpeed => _forwardSpeed;
        public float Sensitivity => _sensitivity;
        public float RoadHalfWidth => _roadHalfWidth;
        public float MaxYawAngle => _maxYawAngle;
        public float YawSmoothTime => _yawSmoothTime;
        public float LateralSmoothTime => _lateralSmoothTime;
    }
}
