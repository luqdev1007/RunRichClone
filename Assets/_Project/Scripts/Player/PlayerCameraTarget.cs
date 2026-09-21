using UnityEngine;

namespace RunRich.Player
{
    public sealed class PlayerCameraTarget : MonoBehaviour
    {
        [SerializeField] private PlayerMover _mover;
        [SerializeField, Range(0f, 60f)] private float _pitch = 18f;

        private void LateUpdate()
        {
            Vector3 roadForward = _mover.RoadForward;
            if (roadForward.sqrMagnitude < Mathf.Epsilon)
                return;

            transform.SetPositionAndRotation(
                _mover.transform.position,
                Quaternion.LookRotation(roadForward, Vector3.up) * Quaternion.Euler(_pitch, 0f, 0f));
        }
    }
}
