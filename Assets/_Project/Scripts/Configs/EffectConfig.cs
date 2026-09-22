using System;
using RunRich.FX;
using UnityEngine;

namespace RunRich.Configs
{
    [CreateAssetMenu(menuName = "RunRich/Effect Config", fileName = "EffectConfig")]
    public sealed class EffectConfig : ScriptableObject
    {
        [SerializeField] private EffectEntry[] _effects;

        public EffectEntry[] Effects => _effects;

        [Serializable]
        public sealed class EffectEntry
        {
            [SerializeField] private EffectKind _kind;
            [SerializeField] private ParticleSystem _prefab;
            [SerializeField] private int _poolSize = 4;

            public EffectKind Kind => _kind;
            public ParticleSystem Prefab => _prefab;
            public int PoolSize => _poolSize;
        }
    }
}
