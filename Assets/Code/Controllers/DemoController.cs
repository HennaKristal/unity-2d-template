using UnityEngine;

public class DemoController : MonoBehaviour
{
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private AudioClip soundEffectClip;

    public void ReloadScene()
    {
        GameManager.Instance.LoadSceneByName("Demo");
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Wait);
        CursorManager.Instance.LockCursorType(2f);
    }

    public void PlayMusicClicked()
    {
        MusicManager.Instance.PlayMusic("ThemeSong", true, 2f);
    }

    public void StopMusicClicked()
    {
        MusicManager.Instance.StopMusic(3f);
    }

    public void OpenSettingsClicked()
    {
        settingsWindow.SetActive(true);
    }

    public void CloseSettingsClicked()
    {
        settingsWindow.SetActive(false);
    }

    public void PlaySoundEffectClicked()
    {
        AudioManager.Instance.PlayUISound(soundEffectClip);
    }
}
