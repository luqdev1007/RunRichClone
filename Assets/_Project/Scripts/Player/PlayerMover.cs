using RunRich.Configs;
using RunRich.Input;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace RunRich.Player
{
    public sealed class PlayerMover : MonoBehaviour
    {
        [SerializeField] private SplineContainer _road;
        [SerializeField] private PlayerMovementConfig _config;
        [SerializeField] private Rigidbody _body;

        private IInputService _input;
        private float _splineLength;
        private float _distance;
        private float _lateralTarget;
        private float _lateralOffset;
        private float _lateralVelocity;
        private float _yaw;
        private float _yawVelocity;

        public float Distance => _distance;
        public float LateralOffset => _lateralOffset;
        public Vector3 RoadForward { get; private set; }

        public void Construct(IInputService input)
        {
            _input = input;
        }

        public void SetRunning(bool running)
        {
            enabled = running;
        }

        private void Awake()
        {
            _splineLength = _road.Spline.GetLength();
            _body.sleepThreshold = 0f;
        }

        private void Update()
        {
            float deltaTime = Time.deltaTime;
            Advance(deltaTime);
            UpdateLateralOffset(deltaTime);
            ApplyTransform(deltaTime);
        }

        private void Advance(float deltaTime)
        {
            _distance = Mathf.Min(_distance + _config.ForwardSpeed * deltaTime, _splineLength);
        }

        private void UpdateLateralOffset(float deltaTime)
        {
            float inputDelta = _input?.HorizontalDelta ?? 0f;
            float halfWidth = _config.RoadHalfWidth;

            _lateralTarget = Mathf.Clamp(_lateralTarget + inputDelta * _config.Sensitivity, -halfWidth, halfWidth);
            _lateralOffset = Mathf.SmoothDamp(_lateralOffset, _lateralTarget, ref _lateralVelocity,
                _config.LateralSmoothTime, Mathf.Infinity, deltaTime);
        }

        private void ApplyTransform(float deltaTime)
        {
            float interpolation = SplineUtility.GetNormalizedInterpolation(_road.Spline, _distance, PathIndexUnit.Distance);
            _road.Evaluate(interpolation, out float3 position, out float3 tangent, out float3 upVector);

            Vector3 forward = ((Vector3)tangent).normalized;
            Vector3 up = ((Vector3)upVector).normalized;
            Vector3 right = Vector3.Cross(up, forward).normalized;
            RoadForward = forward;

            transform.position = (Vector3)position + right * _lateralOffset;
            transform.rotation = Quaternion.LookRotation(forward, up) * Quaternion.Euler(0f, UpdateYaw(deltaTime), 0f);
        }

        private float UpdateYaw(float deltaTime)
        {
            float driftAngle = Mathf.Atan2(_lateralVelocity, _config.ForwardSpeed) * Mathf.Rad2Deg;
            float targetYaw = Mathf.Clamp(driftAngle, -_config.MaxYawAngle, _config.MaxYawAngle);

            _yaw = Mathf.SmoothDampAngle(_yaw, targetYaw, ref _yawVelocity,
                _config.YawSmoothTime, Mathf.Infinity, deltaTime);

            return _yaw;
        }
    }
}
