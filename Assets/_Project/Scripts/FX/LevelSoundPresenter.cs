using System;
using RunRich.Configs;
using RunRich.Level;

namespace RunRich.FX
{
    public sealed class LevelSoundPresenter : IDisposable
    {
        private readonly ChoiceGate[] _gates;
        private readonly FinishDoor[] _doors;
        private readonly AudioService _audio;
        private readonly SoundConfig _config;

        public LevelSoundPresenter(ChoiceGate[] gates, FinishDoor[] doors,
            AudioService audio, SoundConfig config)
        {
            _gates = gates;
            _doors = doors;
            _audio = audio;
            _config = config;

            foreach (var gate in _gates)
                gate.Used += OnGateUsed;

            foreach (var door in _doors)
                door.Reached += OnDoorReached;
        }

        public void Dispose()
        {
            foreach (var gate in _gates)
            {
                if (gate != null)
                    gate.Used -= OnGateUsed;
            }

            foreach (var door in _doors)
            {
                if (door != null)
                    door.Reached -= OnDoorReached;
            }
        }

        private void OnGateUsed(ChoiceGate gate)
        {
            _audio.Play(gate.Reward < 0 ? _config.GateNegative : _config.GatePositive, _config.EffectVolume);
        }

        private void OnDoorReached(FinishDoor door)
        {
            int index = Array.IndexOf(_doors, door);
            var clips = _config.FinishDoors;
            if (index < 0 || clips == null || index >= clips.Length)
                return;

            _audio.Play(clips[index], _config.EffectVolume);
        }
    }
}
