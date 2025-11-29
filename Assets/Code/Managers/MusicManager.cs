using System.Collections;
using UnityEngine;

[System.Serializable]
public class SoundTrack
{
    public string songID;
    public AudioClip audioClip;
    public bool isLooping;
    [Range(0, 1)] public float volume;
}

[RequireComponent(typeof(AudioSource))]
public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance => _instance;

    [SerializeField] private SoundTrack[] soundTracks;
    private AudioSource audioSource;
    private string currentlyPlayingID = "";


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();

        // Mute audio source if there are no preset audio clip
        if (audioSource.clip == null)
        {
            audioSource.volume = 0;
        }
    }

    public void PlayMusic(string songID, bool? shouldLoop = null, float songFadeDuration = 2f, float songVolume = -1f)
    {
        SoundTrack newSong = GetSoundTrackFromID(songID);
        bool songIsLooping = newSong.isLooping;

        if (songVolume == -1f)
        {
            songVolume = newSong.volume;
        }

        if (shouldLoop != null)
        {
            songIsLooping = (bool)shouldLoop;
        }

        if (newSong.audioClip != null)
        {
            currentlyPlayingID = songName;
            StartCoroutine(AnimateMusicCrossfade(newSong.audioClip, songIsLooping, songFadeDuration, songVolume));
        }
        else
        {
            StopMusic();
        }
    }

    public void StopMusic(float fadeDuration = 2f)
    {
        StartCoroutine(AnimateMusicFadeOut(fadeDuration));
    }

    private IEnumerator AnimateMusicCrossfade(AudioClip newAudioClip, bool songIsLooping, float songFadeDuration, float songVolume)
    {
        if (audioSource.isPlaying)
        {
            yield return StartCoroutine(FadeMusicVolume(audioSource.volume, 0, songFadeDuration));
        }

        audioSource.clip = newAudioClip;
        audioSource.loop = songIsLooping;
        audioSource.Play();
        yield return StartCoroutine(FadeMusicVolume(0, songVolume, songFadeDuration));
    }

    private IEnumerator AnimateMusicFadeOut(float fadeDuration)
    {
        yield return StartCoroutine(FadeMusicVolume(audioSource.volume, 0, fadeDuration));
        audioSource.Stop();
    }

    private IEnumerator FadeMusicVolume(float startVolume, float endVolume, float fadeDuration)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / fadeDuration);
            audioSource.volume = Mathf.Lerp(startVolume, endVolume, progress);
            yield return null;
        }

        audioSource.volume = endVolume;
    }

    private SoundTrack GetSoundTrackFromID(string songID)
    {
        foreach (SoundTrack soundTrack in soundTracks)
        {
            if (soundTrack.songID == songID)
            {
                return soundTrack;
            }
        }

        Debug.LogWarning($"Tried to play song with name {name}, but the song was not found.");
        return null;
    }
}
