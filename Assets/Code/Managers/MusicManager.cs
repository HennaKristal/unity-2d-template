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

[RequireComponent(typeof(AudioSource))]
public class MusicManager : Singleton<MusicManager>
{
    [SerializeField] private Song[] soundtrack;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();

        if (audioSource.clip == null)
            audioSource.volume = 0f;
    }

    public void Play(string songID, bool? isLooping = null, float fadeDuration = 2f, float songVolume = -1f)
    {
        Song song = GetSongFromID(songID);

        if (song == null)
        {
            Stop(fadeDuration);
            return;
        }

        bool loopValue = isLooping ?? song.isLooping;
        float volumeValue = (songVolume < 0f) ? song.volume : songVolume;

        StartCoroutine(FadeMusic(
            newClip: song.audioClip,
            isLooping: loopValue,
            targetVolume: volumeValue,
            fadeDuration: fadeDuration
        ));
    }

    public void Stop(float fadeDuration = 2f)
    {
        StartCoroutine(FadeMusic(
            newClip: null,
            isLooping: false,
            targetVolume: 0f,
            fadeDuration: fadeDuration
        ));
    }

    private IEnumerator FadeMusic(AudioClip newClip, bool isLooping, float targetVolume, float fadeDuration)
    {
        // Fade out current music
        yield return FadeVolume(audioSource.volume, 0f, fadeDuration);
        audioSource.Stop();

        // Fade in new music
        if (newClip != null)
        {
            audioSource.clip = newClip;
            audioSource.loop = isLooping;
            audioSource.Play();
            yield return FadeVolume(0f, targetVolume, fadeDuration);
        }
    }

    private IEnumerator FadeVolume(float startVolume, float endVolume, float fadeDuration)
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

    private Song GetSongFromID(string id)
    {
        foreach (Song song in soundtrack)
        {
            if (song.songID == id)
            {
                return song;
            }
        }

        Debug.LogWarning($"Song with ID {id} was not found in the soundtrack.");
        return null;
    }
}
