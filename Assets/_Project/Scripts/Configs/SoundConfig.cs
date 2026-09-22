using UnityEngine;

namespace RunRich.Configs
{
    [CreateAssetMenu(menuName = "RunRich/Sound Config", fileName = "SoundConfig")]
    public sealed class SoundConfig : ScriptableObject
    {
        [SerializeField] private AudioClip[] _moneyPickup;
        [SerializeField] private AudioClip _moneyLoss;
        [SerializeField] private AudioClip _gatePositive;
        [SerializeField] private AudioClip _gateNegative;
        [SerializeField] private AudioClip[] _finishDoors;
        [SerializeField] private AudioClip _win;
        [SerializeField] private AudioClip _lose;
        [SerializeField] private AudioClip[] _footstepsWalk;
        [SerializeField] private AudioClip[] _footstepsCatWalk;

        [SerializeField] private float _walkStepDistance = 1.2f;
        [SerializeField] private float _catWalkStepDistance = 1f;
        [SerializeField, Range(0f, 1f)] private float _effectVolume = 0.9f;
        [SerializeField, Range(0f, 1f)] private float _footstepVolume = 0.45f;

        public AudioClip[] MoneyPickup => _moneyPickup;
        public AudioClip MoneyLoss => _moneyLoss;
        public AudioClip GatePositive => _gatePositive;
        public AudioClip GateNegative => _gateNegative;
        public AudioClip[] FinishDoors => _finishDoors;
        public AudioClip Win => _win;
        public AudioClip Lose => _lose;
        public AudioClip[] FootstepsWalk => _footstepsWalk;
        public AudioClip[] FootstepsCatWalk => _footstepsCatWalk;

        public float WalkStepDistance => _walkStepDistance;
        public float CatWalkStepDistance => _catWalkStepDistance;
        public float EffectVolume => _effectVolume;
        public float FootstepVolume => _footstepVolume;
    }
}
