using UnityEngine;
using UnityEngine.Splines;

namespace RunRich.Level
{
    public sealed class LevelSetup : MonoBehaviour
    {
        [SerializeField] private SplineContainer _road;
        [SerializeField] private FinishTrack _finishTrack;
        [SerializeField] private Transform _playerSpawn;

        public SplineContainer Road => _road;
        public FinishTrack FinishTrack => _finishTrack;
        public Transform PlayerSpawn => _playerSpawn;
    }
}
