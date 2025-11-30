using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : Singleton<AudioManager>
{
    [Header("Audio Source Containers")]
    [SerializeField] private Transform UIContainer;
    [SerializeField] private Transform SFXContainer;
    [SerializeField] private Transform dialogueContainer;
    [SerializeField] private Transform ambienceContainer;
    private readonly List<AudioSource> UIAudioPool = new List<AudioSource>();
    private readonly List<AudioSource> SFXAudioPool = new List<AudioSource>();
    private readonly List<AudioSource> dialogueAudioPool = new List<AudioSource>();
    private readonly List<AudioSource> ambienceAudioPool = new List<AudioSource>();

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup UIMixerGroup;
    [SerializeField] private AudioMixerGroup SFXMixerGroup;
    [SerializeField] private AudioMixerGroup dialogueMixerGroup;
    [SerializeField] private AudioMixerGroup ambienceMixerGroup;

    [Header("Timing Settings")]
    [SerializeField] private float repeatThreshold = 0.1f;
    private readonly Dictionary<AudioClip, float> lastPlayTime = new Dictionary<AudioClip, float>();

    protected override void Awake()
    {
        base.Awake();
        InitializePool(UIContainer, UIAudioPool);
        InitializePool(SFXContainer, SFXAudioPool);
        InitializePool(dialogueContainer, dialogueAudioPool);
        InitializePool(ambienceContainer, ambienceAudioPool);
    }

    private void InitializePool(Transform container, List<AudioSource> pool)
    {
        foreach (Transform child in container)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            pool?.Add(source);
        }
    }

    // -------------------------------------------------------
    // Public API
    // -------------------------------------------------------

    public void PlayUISound(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f, float spatialBlend = 0f, bool loop = false)
    {
        PlayFromPool(clip, UIAudioPool, UIContainer, UIMixerGroup, volume, pitch, delay, spatialBlend, loop);
    }

    public void PlaySFXSound(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f, float spatialBlend = 0f, bool loop = false)
    {
        PlayFromPool(clip, SFXAudioPool, SFXContainer, SFXMixerGroup, volume, pitch, delay, spatialBlend, loop);
    }

    public void PlayVoiceLine(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f, float spatialBlend = 0f, bool loop = false)
    {
        PlayFromPool(clip, dialogueAudioPool, dialogueContainer, dialogueMixerGroup, volume, pitch, delay, spatialBlend, loop);
    }

    public void PlayAmbienceSound(AudioClip clip, float volume = 1f, float pitch = 1f, float delay = 0f, float spatialBlend = 0f, bool loop = false)
    {
        PlayFromPool(clip, dialogueAudioPool, dialogueContainer, dialogueMixerGroup, volume, pitch, delay, spatialBlend, loop);
    }

    // -------------------------------------------------------
    // Core logic
    // -------------------------------------------------------

    private void PlayFromPool(AudioClip clip, List<AudioSource> pool, Transform container, AudioMixerGroup mixerGroup, float volume, float pitch, float delay, float spatialBlend, bool loop)
    {
        if (clip == null)
            return;

        if (IsClipOnCooldown(clip))
            return;

        AudioSource source = GetAvailableSource(pool, container, mixerGroup);
        PrepareSource(source, clip, volume, pitch, spatialBlend, loop);
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

    private AudioSource GetAvailableSource(List<AudioSource> pool, Transform container, AudioMixerGroup mixerGroup)
    {
        foreach (AudioSource source in pool)
        {
            if (!source.isPlaying)
            {
                return source;
            }
        }

        AudioSource newSource = CreateNewSource(container, mixerGroup);
        pool.Add(newSource);
        return newSource;
    }

    private AudioSource CreateNewSource(Transform container, AudioMixerGroup mixerGroup)
    {
        GameObject newObject = new GameObject("Audio Source");
        newObject.transform.SetParent(container);

        AudioSource source = newObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = mixerGroup;
        return source;
    }

    private void PrepareSource(AudioSource source, AudioClip clip, float volume, float pitch, float spatialBlend, bool loop)
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
