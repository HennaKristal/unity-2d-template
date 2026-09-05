using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioController : MonoBehaviour
{
    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;
    private bool isInitializing = true;

    [Header("Sliders")]
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider ambientVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private Slider UIVolumeSlider;
    [SerializeField] private Slider dialogueVolumeSlider;

    [Header("Texts")]
    [SerializeField] private TextMeshProUGUI musicVolumeSliderText;
    [SerializeField] private TextMeshProUGUI ambientVolumeSliderText;
    [SerializeField] private TextMeshProUGUI SFXVolumeSliderText;
    [SerializeField] private TextMeshProUGUI UIVolumeSliderText;
    [SerializeField] private TextMeshProUGUI dialogueVolumeSliderText;

    [Header("Sounds")]
    [SerializeField] private AudioClip sliderChangeSound;

    private void Start()
    {
        LoadVolume("MusicVolume", musicVolumeSlider, musicVolumeSliderText);
        LoadVolume("AmbientVolume", ambientVolumeSlider, ambientVolumeSliderText);
        LoadVolume("SFXVolume", SFXVolumeSlider, SFXVolumeSliderText);
        LoadVolume("DialogueVolume", dialogueVolumeSlider, dialogueVolumeSliderText);
        LoadVolume("UIVolume", UIVolumeSlider, UIVolumeSliderText);

        isInitializing = false;
    }

    public void OnVolumeSliderChanged()
    {
        if (isInitializing)
        {
            return;
        }

        ApplyVolume("MusicVolume", musicVolumeSlider, musicVolumeSliderText);
        ApplyVolume("AmbientVolume", ambientVolumeSlider, ambientVolumeSliderText);
        ApplyVolume("SFXVolume", SFXVolumeSlider, SFXVolumeSliderText);
        ApplyVolume("DialogueVolume", dialogueVolumeSlider, dialogueVolumeSliderText);
        ApplyVolume("UIVolume", UIVolumeSlider, UIVolumeSliderText);

        PlayerPrefs.Save();
    }

    private void LoadVolume(string parameter, Slider slider, TextMeshProUGUI label)
    {
        float value = PlayerPrefs.GetFloat(parameter, 0.5f);
        slider.value = value;
        label.text = Mathf.Ceil(value * 100f).ToString() + "%";
        audioMixer.SetFloat(parameter, LinearToDecibel(value));
    }

    private void ApplyVolume(string parameter, Slider slider, TextMeshProUGUI label)
    {
        float value = slider.value;
        label.text = Mathf.Ceil(value * 100f).ToString() + "%";
        PlayerPrefs.SetFloat(parameter, value);
        audioMixer.SetFloat(parameter, LinearToDecibel(value));
    }

    private float LinearToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }

    public void PlayAmbientSliderChangeSound()
    {
        if (isInitializing) return;
        AudioManager.Instance.PlayAmbienceSound(sliderChangeSound);
    }

    public void PlaySFXSliderChangeSound()
    {
        if (isInitializing) return;
        AudioManager.Instance.PlaySFXSound(sliderChangeSound); 
    }

    public void PlayUISliderChangeSound()
    {
        if (isInitializing) return;
        AudioManager.Instance.PlayUISound(sliderChangeSound);
    }

    public void PlayDialogueSliderChangeSound()
    {
        if (isInitializing) return;
        AudioManager.Instance.PlayVoiceLine(sliderChangeSound);
    }
}
