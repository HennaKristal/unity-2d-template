using System.Collections.Generic;
using UnityEngine;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Source Containers")]
    [SerializeField] private Transform UIContainer;
    [SerializeField] private Transform SFXContainer;
    [SerializeField] private Transform dialogueContainer;
    private readonly List<AudioSource> UIAudioPool = new List<AudioSource>();
    private readonly List<AudioSource> SFXAudioPool = new List<AudioSource>();
    private readonly List<AudioSource> dialogueAudioPool = new List<AudioSource>();

    [Header("Timing Settings")]
    [SerializeField] private float repeatThreshold = 0.1f;
    private readonly Dictionary<AudioClip, float> lastPlayTime = new Dictionary<AudioClip, float>();

    protected override void Awake()
    {
        base.Awake();
        InitializePool(UIContainer, UIAudioPool);
        InitializePool(SFXContainer, SFXAudioPool);
        InitializePool(dialogueContainer, dialogueAudioPool);
    }

    private void InitializePool(Transform container, List<AudioSource> pool)
    {
        foreach (Transform child in container)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            pool?.Add(source);
        }
    }

    public void PlayUISound(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f, float spatialBlend = 0f, bool loop = false)
    {
        PlayFromPool(clip, UIAudioPool, UIContainer, volume, pitch, delay, spatialBlend, loop);
    }

    public void PlaySFXSound(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f, float spatialBlend = 0f, bool loop = false)
    {
        PlayFromPool(clip, SFXAudioPool, SFXContainer, volume, pitch, delay, spatialBlend, loop);
    }

    public void PlayVoiceLine(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f, float spatialBlend = 0f, bool loop = false)
    {
        PlayFromPool(clip, dialogueAudioPool, dialogueContainer, volume, pitch, delay, spatialBlend, loop);
    }

    private void PlayFromPool(AudioClip clip, List<AudioSource> pool, Transform container, float volume, float pitch, float delay, float spatialBlend, bool loop)
    {
        if (clip == null)
            return;

        if (IsClipOnCooldown(clip))
            return;

        AudioSource source = GetAvailableSource(pool, container);
        PrepareSource(source, clip, volume, pitch, delay, spatialBlend, loop);
        PlaySource(source, delay);
    }

    private bool IsClipOnCooldown(AudioClip clip)
    {
        float currentTime = Time.time;

        if (lastPlayTime.TryGetValue(clip, out float lastTime))
        {
            if (currentTime - lastTime < repeatThreshold)
            {
                return true;
            }
        }

        lastPlayTime[clip] = currentTime;
        return false;
    }

    private AudioSource GetAvailableSource(List<AudioSource> pool, Transform container)
    {
        foreach (AudioSource source in pool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        AudioSource newSource = CreateNewSource(container);
        pool.Add(newSource);
        return newSource;
    }

    private AudioSource CreateNewSource(Transform container)
    {
        GameObject newObject = new GameObject("Audio Source");
        newObject.transform.SetParent(container);
        return newObject.AddComponent<AudioSource>();
    }

    private void PrepareSource(AudioSource source, AudioClip clip, float volume, float pitch, float delay, float spatialBlend, bool loop)
    {
        source.clip = clip;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = spatialBlend;
        source.loop = loop;
    }

    private void PlaySource(AudioSource source, float delay)
    {
        if (delay > 0f)
        {
            source.PlayDelayed(delay);
        }
        else
        {
            source.Play();
        }
    }
}
