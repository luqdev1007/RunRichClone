using System.Collections;
using UnityEngine;
using UnityEngine.Pool;

namespace RunRich.FX
{
    public sealed class AudioService : MonoBehaviour
    {
        private const int DefaultCapacity = 8;
        private const int MaxPoolSize = 24;

        private ObjectPool<AudioSource> _pool;

        public void Play(AudioClip clip, float volume)
        {
            if (clip == null)
                return;

            var source = _pool.Get();
            source.clip = clip;
            source.volume = volume;
            source.Play();
            StartCoroutine(ReleaseWhenFinished(source, clip.length));
        }

        public void PlayRandom(AudioClip[] clips, float volume)
        {
            if (clips == null || clips.Length == 0)
                return;

            Play(clips[Random.Range(0, clips.Length)], volume);
        }

        private void Awake()
        {
            _pool = new ObjectPool<AudioSource>(
                Create, OnGet, OnRelease, OnDestroySource, true, DefaultCapacity, MaxPoolSize);
        }

        private void OnDestroy()
        {
            _pool?.Dispose();
        }

        private AudioSource Create()
        {
            var go = new GameObject("PooledAudioSource");
            go.transform.SetParent(transform, false);
            var source = go.AddComponent<AudioSource>();
            source.playOnAwake = false;
            return source;
        }

        private static void OnGet(AudioSource source)
        {
            source.gameObject.SetActive(true);
        }

        private static void OnRelease(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            source.gameObject.SetActive(false);
        }

        private static void OnDestroySource(AudioSource source)
        {
            if (source != null)
                Destroy(source.gameObject);
        }

        private IEnumerator ReleaseWhenFinished(AudioSource source, float length)
        {
            yield return new WaitForSeconds(length);
            _pool.Release(source);
        }
    }
}
