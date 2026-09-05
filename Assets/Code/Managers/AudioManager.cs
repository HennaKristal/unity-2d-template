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
    [SerializeField] private Transform customContainer;

    private readonly List<AudioSource> UIAudioPool = new List<AudioSource>();
    private readonly List<AudioSource> SFXAudioPool = new List<AudioSource>();
    private readonly List<AudioSource> dialogueAudioPool = new List<AudioSource>();
    private readonly List<AudioSource> ambienceAudioPool = new List<AudioSource>();
    private readonly Dictionary<string, AudioSource> persistentSources = new Dictionary<string, AudioSource>();

    [Header("Audio Mixer Groups")]
    [SerializeField] private AudioMixerGroup UIMixerGroup;
    [SerializeField] private AudioMixerGroup SFXMixerGroup;
    [SerializeField] private AudioMixerGroup dialogueMixerGroup;
    [SerializeField] private AudioMixerGroup ambienceMixerGroup;

    [Header("Distance Settings")]
    [SerializeField] private float defaultSFXMaxDistance = 10.0f;
    [SerializeField] private float defaultDialogueMaxDistance = 10.0f;

    [Header("Timing Settings")]
    [SerializeField] private float repeatThreshold = 0.1f;

    private readonly Dictionary<AudioClip, float> lastPlayTime = new Dictionary<AudioClip, float>();

    private Transform playerTransform;

    protected override void Awake()
    {
        base.Awake();

        InitializePool(UIContainer, UIAudioPool);
        InitializePool(SFXContainer, SFXAudioPool);
        InitializePool(dialogueContainer, dialogueAudioPool);
        InitializePool(ambienceContainer, ambienceAudioPool);
    }

    private void OnEnable()
    {
        GameManager.Instance.OnPlayerTransformChanged += OnPlayerTransformChanged;
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnPlayerTransformChanged -= OnPlayerTransformChanged;
        }
    }

    private void OnPlayerTransformChanged(Transform playerTransform)
    {
        this.playerTransform = playerTransform;
    }

    private void InitializePool(Transform container, List<AudioSource> pool)
    {
        foreach (Transform child in container)
        {
            AudioSource source = child.GetComponent<AudioSource>();
            pool.Add(source);
        }
    }

    // -------------------------------------------------------
    // Public API
    // -------------------------------------------------------

    public void PlayUISound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f, float delay = 0.0f, float spatialBlend = 0.0f, bool loop = false)
    {
        PlayFromPool(clip, UIAudioPool, UIContainer, UIMixerGroup, volume, pitch, delay, spatialBlend, loop);
    }

    public void PlaySFXSound(AudioClip clip, Vector3? worldPosition = null, float maxDistance = -1.0f, float volume = 1.0f, float pitch = 1.0f, float delay = 0.0f, float spatialBlend = 0.0f, bool loop = false, AudioMixerGroup mixerGroup = null)
    {
        AudioMixerGroup finalMixerGroup = mixerGroup != null ? mixerGroup : SFXMixerGroup;
        float finalVolume = volume;

        if (worldPosition.HasValue)
        {
            float finalMaxDistance = maxDistance > 0.0f ? maxDistance : defaultSFXMaxDistance;
            finalVolume = GetDistanceVolume(worldPosition.Value, volume, finalMaxDistance);

            if (finalVolume <= 0.0f)
            {
                return;
            }
        }

        PlayFromPool(clip, SFXAudioPool, SFXContainer, finalMixerGroup, finalVolume, pitch, delay, spatialBlend, loop);
    }

    public void PlayVoiceLine(AudioClip clip, Vector3? worldPosition = null, float maxDistance = -1.0f, float volume = 1.0f, float pitch = 1.0f, float delay = 0.0f, float spatialBlend = 0.0f, bool loop = false)
    {
        float finalVolume = volume;

        if (worldPosition.HasValue)
        {
            float finalMaxDistance = maxDistance > 0.0f ? maxDistance : defaultDialogueMaxDistance;
            finalVolume = GetDistanceVolume(worldPosition.Value, volume, finalMaxDistance);

            if (finalVolume <= 0.0f)
            {
                return;
            }
        }

        PlayFromPool(clip, dialogueAudioPool, dialogueContainer, dialogueMixerGroup, finalVolume, pitch, delay, spatialBlend, loop);
    }

    public void PlayAmbienceSound(AudioClip clip, float volume = 1.0f, float pitch = 1.0f, float delay = 0.0f, float spatialBlend = 0.0f, bool loop = false)
    {
        PlayFromPool(clip, ambienceAudioPool, ambienceContainer, ambienceMixerGroup, volume, pitch, delay, spatialBlend, loop);
    }

    // Distance
    private float GetDistanceVolume(Vector3 worldPosition, float volume, float maxDistance)
    {
        if (playerTransform == null)
        {
            return volume;
        }

        float distanceToPlayer = Vector2.Distance(worldPosition, playerTransform.position);
        float distanceVolume = 1.0f - Mathf.Clamp01(distanceToPlayer / maxDistance);

        return volume * distanceVolume;
    }

    // -------------------------------------------------------
    // Core logic
    // -------------------------------------------------------
    private void PlayFromPool(AudioClip clip, List<AudioSource> pool, Transform container, AudioMixerGroup mixerGroup, float volume, float pitch, float delay, float spatialBlend, bool loop)
    {
        if (clip == null)
        {
            return;
        }

        if (IsClipOnCooldown(clip))
        {
            return;
        }

        AudioSource source = GetAvailableSource(pool, container, mixerGroup);

        PrepareSource(source, clip, mixerGroup, volume, pitch, spatialBlend, loop);
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

    private void PrepareSource(AudioSource source, AudioClip clip, AudioMixerGroup mixerGroup, float volume, float pitch, float spatialBlend, bool loop)
    {
        source.clip = clip;
        source.outputAudioMixerGroup = mixerGroup;
        source.volume = volume;
        source.pitch = pitch;
        source.spatialBlend = spatialBlend;
        source.loop = loop;
    }

    private void PlaySource(AudioSource source, float delay)
    {
        if (delay > 0.0f)
        {
            source.PlayDelayed(delay);
        }
        else
        {
            source.Play();
        }
    }

    // -------------------------------------------------------
    // Custom Persistent Audio Sources
    // -------------------------------------------------------

    public AudioSource CreatePersistentAudioSource(string identifier, AudioMixerGroup mixerGroup, AudioClip clip)
    {
        if (string.IsNullOrWhiteSpace(identifier))
        {
            return null;
        }

        if (persistentSources.ContainsKey(identifier))
        {
            return persistentSources[identifier];
        }

        GameObject newObject = new GameObject("Audio Source (" + identifier + ")");
        newObject.transform.SetParent(customContainer);

        AudioSource source = newObject.AddComponent<AudioSource>();
        source.outputAudioMixerGroup = mixerGroup;
        source.clip = clip;

        persistentSources.Add(identifier, source);

        return source;
    }

    public AudioSource GetPersistentAudioSource(string identifier)
    {
        if (!persistentSources.TryGetValue(identifier, out AudioSource source))
        {
            return null;
        }

        return source;
    }

    public void DeletePersistentAudioSource(string identifier)
    {
        if (!persistentSources.TryGetValue(identifier, out AudioSource source))
        {
            return;
        }

        persistentSources.Remove(identifier);
        Destroy(source.gameObject);
    }
}
