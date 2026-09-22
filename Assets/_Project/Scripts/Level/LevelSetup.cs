using UnityEngine;
using UnityEngine.Splines;

namespace RunRich.Level
{
    public sealed class LevelSetup : MonoBehaviour
    {
        [SerializeField] private SplineContainer _road;
        [SerializeField] private FinishTrack _finishTrack;
        [SerializeField] private ChoiceGate[] _gates;
        [SerializeField] private Transform _playerSpawn;

        public SplineContainer Road => _road;
        public FinishTrack FinishTrack => _finishTrack;
        public ChoiceGate[] Gates => _gates;
        public Transform PlayerSpawn => _playerSpawn;
    }
}
