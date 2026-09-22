using RunRich.Configs;
using RunRich.Player;
using UnityEngine;

namespace RunRich.FX
{
    public sealed class FootstepPlayer : MonoBehaviour
    {
        private const int WalkGaitId = 0;

        private PlayerMover _mover;
        private PlayerView _view;
        private AudioService _audio;
        private SoundConfig _config;
        private float _nextStepDistance;

        public void Construct(PlayerMover mover, PlayerView view, AudioService audio, SoundConfig config)
        {
            _mover = mover;
            _view = view;
            _audio = audio;
            _config = config;
            _nextStepDistance = _mover.Distance + StepDistance();
        }

        private void Update()
        {
            if (_mover == null || !_mover.enabled)
                return;

            if (_mover.Distance < _nextStepDistance)
                return;

            bool catWalk = _view.CurrentGaitId != WalkGaitId;
            _audio.PlayRandom(catWalk ? _config.FootstepsCatWalk : _config.FootstepsWalk, _config.FootstepVolume);
            _nextStepDistance = _mover.Distance + StepDistance();
        }

        private float StepDistance()
        {
            return _view.CurrentGaitId == WalkGaitId
                ? _config.WalkStepDistance
                : _config.CatWalkStepDistance;
        }
    }
}
