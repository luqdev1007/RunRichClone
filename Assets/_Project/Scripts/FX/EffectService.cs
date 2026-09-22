using System.Collections;
using System.Collections.Generic;
using RunRich.Configs;
using UnityEngine;
using UnityEngine.Pool;

namespace RunRich.FX
{
    public sealed class EffectService : MonoBehaviour
    {
        private const int MaxPoolSize = 16;

        private readonly Dictionary<EffectKind, ObjectPool<ParticleSystem>> _pools =
            new Dictionary<EffectKind, ObjectPool<ParticleSystem>>();

        public void Construct(EffectConfig config)
        {
            foreach (var entry in config.Effects)
            {
                if (entry.Prefab == null || _pools.ContainsKey(entry.Kind))
                    continue;

                var prefab = entry.Prefab;
                _pools.Add(entry.Kind, new ObjectPool<ParticleSystem>(
                    () => Create(prefab),
                    OnGet, OnRelease, OnDestroyEffect,
                    true, entry.PoolSize, MaxPoolSize));
            }
        }

        public void Play(EffectKind kind, Vector3 position, Quaternion rotation)
        {
            var effect = Take(kind, position, rotation);
            if (effect != null)
                effect.Play(true);
        }

        public void Play(EffectKind kind, Vector3 position, Quaternion rotation, Color color)
        {
            var effect = Take(kind, position, rotation);
            if (effect == null)
                return;

            foreach (var system in effect.GetComponentsInChildren<ParticleSystem>(true))
            {
                var main = system.main;
                main.startColor = color;
            }

            effect.Play(true);
        }

        private ParticleSystem Take(EffectKind kind, Vector3 position, Quaternion rotation)
        {
            ObjectPool<ParticleSystem> pool;
            if (!_pools.TryGetValue(kind, out pool))
                return null;

            var effect = pool.Get();
            effect.transform.SetPositionAndRotation(position, rotation);
            StartCoroutine(ReleaseWhenFinished(pool, effect));
            return effect;
        }

        private void OnDestroy()
        {
            foreach (var pool in _pools.Values)
                pool.Dispose();

            _pools.Clear();
        }

        private ParticleSystem Create(ParticleSystem prefab)
        {
            var instance = Instantiate(prefab, transform);
            instance.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            return instance;
        }

        private static void OnGet(ParticleSystem effect)
        {
            effect.gameObject.SetActive(true);
        }

        private static void OnRelease(ParticleSystem effect)
        {
            effect.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
            effect.gameObject.SetActive(false);
        }

        private static void OnDestroyEffect(ParticleSystem effect)
        {
            if (effect != null)
                Destroy(effect.gameObject);
        }

        private IEnumerator ReleaseWhenFinished(ObjectPool<ParticleSystem> pool, ParticleSystem effect)
        {
            yield return null;

            while (effect != null && effect.IsAlive(true))
                yield return null;

            if (effect != null)
                pool.Release(effect);
        }
    }
}
