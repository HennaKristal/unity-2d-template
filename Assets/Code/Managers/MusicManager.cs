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
public class MusicManager : MonoBehaviour
{
    private static MusicManager instance;
    public static MusicManager Instance => instance;
    [SerializeField] private Song[] soundTrack;
    private AudioSource audioSource;
    private string currentlyPlayingID = "";

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        if (audioSource.clip == null)
        {
            audioSource.volume = 0f;
        }
    }

    public void Play(string songID, bool? isLooping = null, float fadeDuration = 2f, float songVolume = -1f)
    {
        Song song = GetSongFromID(songID);

        if (song == null)
        {
            Stop(fadeDuration);
            return;
        }

        bool finalLoopValue = isLooping ?? song.isLooping;
        float finalVolumeValue = (songVolume < 0f) ? song.volume : songVolume;

        currentlyPlayingID = songID;
        StartCoroutine(AnimateMusicCrossfade(song.audioClip, finalLoopValue, fadeDuration, finalVolumeValue));
    }

    public void Stop(float fadeDuration = 2f)
    {
        StartCoroutine(AnimateMusicFadeOut(fadeDuration));
    }

    private IEnumerator AnimateMusicCrossfade(AudioClip newClip, bool isLooping, float fadeDuration, float targetVolume)
    {
        if (audioSource.isPlaying)
        {
            yield return FadeMusicVolume(audioSource.volume, 0f, fadeDuration);
        }

        audioSource.clip = newClip;
        audioSource.loop = isLooping;
        audioSource.Play();

        yield return FadeMusicVolume(0f, targetVolume, fadeDuration);
    }

    private IEnumerator AnimateMusicFadeOut(float fadeDuration)
    {
        yield return FadeMusicVolume(audioSource.volume, 0f, fadeDuration);
        audioSource.Stop();
    }

    private IEnumerator FadeMusicVolume(float startVolume, float endVolume, float fadeDuration)
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
        foreach (Song song in soundTrack)
        {
            if (song.songID == id)
            {
                return song;
            }
        }

        Debug.LogWarning($"Tried to get a song with ID {id}, but it was not found in music manager sound track.");
        return null;
    }
}
