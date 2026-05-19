using UnityEngine;
using System.Collections.Generic;

namespace ChronicSurvival.Core
{
    /// <summary>
    /// Manages all audio in the game (music and SFX)
    /// Singleton pattern
    /// </summary>
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;
        [SerializeField] private int sfxPoolSize = 10;

        [Header("Volume Settings")]
        [SerializeField] [Range(0f, 1f)] private float masterVolume = 1f;
        [SerializeField] [Range(0f, 1f)] private float musicVolume = 0.7f;
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 1f;

        [Header("Debug")]
        [SerializeField] private bool debugMode = false;

        // SFX pool
        private List<AudioSource> sfxPool = new List<AudioSource>();
        private int currentSFXIndex = 0;

        // Current music
        private AudioClip currentMusic;

        private void Awake()
        {
            // Singleton
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            Initialize();
        }

        private void Initialize()
        {
            // Create music source if not assigned
            if (musicSource == null)
            {
                GameObject musicObj = new GameObject("MusicSource");
                musicObj.transform.SetParent(transform);
                musicSource = musicObj.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }

            // Create SFX source if not assigned
            if (sfxSource == null)
            {
                GameObject sfxObj = new GameObject("SFXSource");
                sfxObj.transform.SetParent(transform);
                sfxSource = sfxObj.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }

            // Create SFX pool
            for (int i = 0; i < sfxPoolSize; i++)
            {
                GameObject sfxObj = new GameObject($"SFXPool_{i}");
                sfxObj.transform.SetParent(transform);
                AudioSource source = sfxObj.AddComponent<AudioSource>();
                source.playOnAwake = false;
                sfxPool.Add(source);
            }

            ApplyVolume();

            if (debugMode) Debug.Log("[AudioManager] Initialized");
        }

        #region Music

        public void PlayMusic(AudioClip clip, bool loop = true, float fadeTime = 1f)
        {
            if (clip == null) return;

            if (currentMusic == clip && musicSource.isPlaying) return;

            if (fadeTime > 0 && musicSource.isPlaying)
            {
                StartCoroutine(FadeOutAndPlayNew(clip, loop, fadeTime));
            }
            else
            {
                musicSource.clip = clip;
                musicSource.loop = loop;
                musicSource.Play();
                currentMusic = clip;
            }

            if (debugMode) Debug.Log($"[AudioManager] Playing music: {clip.name}");
        }

        public void StopMusic(float fadeTime = 1f)
        {
            if (fadeTime > 0)
            {
                StartCoroutine(FadeOut(musicSource, fadeTime));
            }
            else
            {
                musicSource.Stop();
            }

            currentMusic = null;
        }

        public void PauseMusic()
        {
            musicSource.Pause();
        }

        public void ResumeMusic()
        {
            musicSource.UnPause();
        }

        private System.Collections.IEnumerator FadeOut(AudioSource source, float duration)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return null;
            }

            source.Stop();
            source.volume = startVolume;
        }

        private System.Collections.IEnumerator FadeOutAndPlayNew(AudioClip newClip, bool loop, float duration)
        {
            yield return FadeOut(musicSource, duration);
            PlayMusic(newClip, loop, 0f);
            yield return FadeIn(musicSource, duration);
        }

        private System.Collections.IEnumerator FadeIn(AudioSource source, float duration)
        {
            float targetVolume = musicVolume * masterVolume;
            source.volume = 0f;
            float elapsed = 0f;

            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                source.volume = Mathf.Lerp(0f, targetVolume, elapsed / duration);
                yield return null;
            }

            source.volume = targetVolume;
        }

        #endregion

        #region SFX

        public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null) return;

            // Get next available source from pool
            AudioSource source = sfxPool[currentSFXIndex];
            currentSFXIndex = (currentSFXIndex + 1) % sfxPool.Count;

            source.volume = sfxVolume * masterVolume * volumeMultiplier;
            source.PlayOneShot(clip);

            if (debugMode) Debug.Log($"[AudioManager] Playing SFX: {clip.name}");
        }

        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volumeMultiplier = 1f)
        {
            if (clip == null) return;

            AudioSource.PlayClipAtPoint(clip, position, sfxVolume * masterVolume * volumeMultiplier);

            if (debugMode) Debug.Log($"[AudioManager] Playing SFX at position: {clip.name}");
        }

        public void StopAllSFX()
        {
            foreach (var source in sfxPool)
            {
                source.Stop();
            }
        }

        #endregion

        #region Volume Control

        public void SetMasterVolume(float volume)
        {
            masterVolume = Mathf.Clamp01(volume);
            ApplyVolume();
        }

        public void SetMusicVolume(float volume)
        {
            musicVolume = Mathf.Clamp01(volume);
            ApplyVolume();
        }

        public void SetSFXVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
            ApplyVolume();
        }

        private void ApplyVolume()
        {
            if (musicSource != null)
                musicSource.volume = musicVolume * masterVolume;

            if (sfxSource != null)
                sfxSource.volume = sfxVolume * masterVolume;

            foreach (var source in sfxPool)
            {
                source.volume = sfxVolume * masterVolume;
            }
        }

        public float GetMasterVolume() => masterVolume;
        public float GetMusicVolume() => musicVolume;
        public float GetSFXVolume() => sfxVolume;

        #endregion
    }
}
