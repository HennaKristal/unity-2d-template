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
    [SerializeField] private AudioSource ambienceAudioSource;

    [Header("Soundtracks")]
    [SerializeField] private Song[] soundtrack;
    [SerializeField] private Song[] ambienceSoundTrack;

    private Coroutine musicFadeCoroutine;
    private Coroutine ambienceFadeCoroutine;


    protected override void Awake()
    {
        base.Awake();

        // Start with silence if no clip is assigned
        if (musicAudioSource.clip == null)
            musicAudioSource.volume = 0f;

        if (ambienceAudioSource.clip == null)
            ambienceAudioSource.volume = 0f;
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
        float volumeValue = songVolume < 0f ? song.volume : songVolume;

        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        musicFadeCoroutine = StartCoroutine(FadeAudio(
            musicAudioSource,
            song.audioClip,
            loopValue,
            volumeValue,
            fadeDuration
        ));
    }


    public void StopMusic(float fadeDuration = 2f)
    {
        if (musicFadeCoroutine != null)
        {
            StopCoroutine(musicFadeCoroutine);
        }

        musicFadeCoroutine = StartCoroutine(FadeAudio(
            musicAudioSource,
            null,
            false,
            0f,
            fadeDuration
        ));
    }


    public void PlayAmbience(string songID, bool? isLooping = null, float fadeDuration = 2f, float songVolume = -1f)
    {
        Song song = GetSongFromID(songID, ambienceSoundTrack);

        if (song == null)
        {
            StopAmbience(fadeDuration);
            return;
        }

        bool loopValue = isLooping ?? song.isLooping;
        float volumeValue = songVolume < 0f ? song.volume : songVolume;

        if (ambienceFadeCoroutine != null)
        {
            StopCoroutine(ambienceFadeCoroutine);
        }

        ambienceFadeCoroutine = StartCoroutine(FadeAudio(
            ambienceAudioSource,
            song.audioClip,
            loopValue,
            volumeValue,
            fadeDuration
        ));
    }


    public void StopAmbience(float fadeDuration = 2f)
    {
        if (ambienceFadeCoroutine != null)
        {
            StopCoroutine(ambienceFadeCoroutine);
        }

        ambienceFadeCoroutine = StartCoroutine(FadeAudio(
            ambienceAudioSource,
            null,
            false,
            0f,
            fadeDuration
        ));
    }


    // -------------------------------------------------------
    // Core logic
    // -------------------------------------------------------

    private IEnumerator FadeAudio(AudioSource audioSource, AudioClip newClip, bool isLooping, float targetVolume, float fadeDuration)
    {
        if (fadeDuration <= 0f)
        {
            audioSource.Stop();
            audioSource.clip = newClip;
            audioSource.volume = targetVolume;

            if (newClip != null)
            {
                audioSource.loop = isLooping;
                audioSource.Play();
            }

            yield break;
        }

        float startVolume = audioSource.volume;

        if (audioSource.clip != null && audioSource.isPlaying)
        {
            yield return FadeVolume(
                audioSource,
                startVolume,
                0f,
                fadeDuration
            );
        }

        audioSource.Stop();
        audioSource.clip = newClip;

        if (newClip == null)
        {
            audioSource.volume = 0f;
            yield break;
        }

        audioSource.loop = isLooping;
        audioSource.volume = 0f;
        audioSource.Play();

        yield return FadeVolume(audioSource, 0f, targetVolume, fadeDuration);
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
