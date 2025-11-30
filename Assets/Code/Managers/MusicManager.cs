using System.Collections;
using UnityEngine;

[System.Serializable]
public class Song
{
    public string songID;
    public AudioClip audioClip;
    public bool isLooping;
    [Range(0, 1)] public float volume;
}

public class MusicManager : Singleton<MusicManager>
{
    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource ambianceAudioSource;

    [Header("Soundtracks")]
    [SerializeField] private Song[] soundtrack;
    [SerializeField] private Song[] ambianceSoundTrack;

    protected override void Awake()
    {
        base.Awake();

        // Start with silence if no clip is assigned
        if (musicAudioSource.clip == null)
            musicAudioSource.volume = 0f;

        if (ambianceAudioSource.clip == null)
            ambianceAudioSource.volume = 0f;
    }

    // -------------------------------------------------------
    // Public API
    // -------------------------------------------------------

    public void PlayMusic(string songID, bool? isLooping = null, float fadeDuration = 2f, float songVolume = -1f)
    {
        Song song = GetSongFromID(songID, soundtrack);

        if (song == null)
        {
            StopMusic(fadeDuration);
            return;
        }

        bool loopValue = isLooping ?? song.isLooping;
        float volumeValue = (songVolume < 0f) ? song.volume : songVolume;

        StartCoroutine(FadeAudio(
            audioSource: musicAudioSource,
            newClip: song.audioClip,
            isLooping: loopValue,
            targetVolume: volumeValue,
            fadeDuration: fadeDuration
        ));
    }

    public void StopMusic(float fadeDuration = 2f)
    {
        StartCoroutine(FadeAudio(
            audioSource: musicAudioSource,
            newClip: null,
            isLooping: false,
            targetVolume: 0f,
            fadeDuration: fadeDuration
        ));
    }

    public void PlayAmbiance(string songID, bool? isLooping = null, float fadeDuration = 2f, float songVolume = -1f)
    {
        Song song = GetSongFromID(songID, ambianceSoundTrack);

        if (song == null)
        {
            StopAmbiance(fadeDuration);
            return;
        }

        bool loopValue = isLooping ?? song.isLooping;
        float volumeValue = (songVolume < 0f) ? song.volume : songVolume;

        StartCoroutine(FadeAudio(
            audioSource: ambianceAudioSource,
            newClip: song.audioClip,
            isLooping: loopValue,
            targetVolume: volumeValue,
            fadeDuration: fadeDuration
        ));
    }

    public void StopAmbiance(float fadeDuration = 2f)
    {
        StartCoroutine(FadeAudio(
            audioSource: ambianceAudioSource,
            newClip: null,
            isLooping: false,
            targetVolume: 0f,
            fadeDuration: fadeDuration
        ));
    }

    // -------------------------------------------------------
    // Core logic
    // -------------------------------------------------------

    private IEnumerator FadeAudio(AudioSource audioSource, AudioClip newClip, bool isLooping, float targetVolume, float fadeDuration)
    {
        float startVolume = audioSource.volume;

        // Fade out current audio
        if (audioSource.clip != null && startVolume > 0f)
        {
            yield return FadeVolume(audioSource, startVolume, 0f, fadeDuration);
            audioSource.Stop();
            audioSource.clip = null;
        }

        // Fade in new audio
        if (newClip != null)
        {
            audioSource.clip = newClip;
            audioSource.loop = isLooping;
            audioSource.Play();
            yield return FadeVolume(audioSource, 0f, targetVolume, fadeDuration);
        }
    }

    private IEnumerator FadeVolume(AudioSource audioSource, float startVolume, float endVolume, float fadeDuration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, progress);
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    // -------------------------------------------------------
    // Helpers
    // -------------------------------------------------------

    private Song GetSongFromID(string id, Song[] collection)
    {
        foreach (Song song in collection)
        {
            if (song.songID == id)
            {
                return song;
            }
        }

        Debug.LogWarning($"Song with ID {id} was not found.");
        return null;
    }
}
