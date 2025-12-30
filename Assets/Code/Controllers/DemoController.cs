using UnityEngine;

public class DemoController : MonoBehaviour
{
    [SerializeField] private GameObject demoButtons;
    [SerializeField] private GameObject settingsWindow;
    [SerializeField] private GameObject RemapKeysWindow;
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
        demoButtons.SetActive(false);
        settingsWindow.SetActive(true);
    }

    public void CloseSettingsClicked()
    {
        settingsWindow.SetActive(false);
        demoButtons.SetActive(true);
    }

    public void OpenRemapKeysClicked()
    {
        demoButtons.SetActive(false);
        RemapKeysWindow.SetActive(true);
    }

    public void CloseRemapKeysClicked()
    {
        RemapKeysWindow.SetActive(false);
        demoButtons.SetActive(true);
    }

    public void PlaySoundEffectClicked()
    {
        AudioManager.Instance.PlayUISound(soundEffectClip);
    }
}
