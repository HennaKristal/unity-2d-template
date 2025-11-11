using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider MusicVolumeSlider;
    [SerializeField] private Slider AmbientVolumeSlider;
    [SerializeField] private Slider SFXVolumeSlider;
    [SerializeField] private Slider DialogueVolumeSlider;
    [SerializeField] private Slider UIVolumeSlider;


    private void Start()
    {
        LoadVolume("MusicVolume", MusicVolumeSlider);
        LoadVolume("AmbientVolume", AmbientVolumeSlider);
        LoadVolume("SFXVolume", SFXVolumeSlider);
        LoadVolume("DialogueVolume", DialogueVolumeSlider);
        LoadVolume("UIVolume", UIVolumeSlider);
    }


    public void OnVolumeSliderChanged()
    {
        ApplyVolume("MusicVolume", MusicVolumeSlider);
        ApplyVolume("AmbientVolume", AmbientVolumeSlider);
        ApplyVolume("SFXVolume", SFXVolumeSlider);
        ApplyVolume("DialogueVolume", DialogueVolumeSlider);
        ApplyVolume("UIVolume", UIVolumeSlider);
    }


    private void LoadVolume(string parameter, Slider slider)
    {
        float value = PlayerPrefs.GetFloat(parameter, 1f);
        slider.value = value;
        audioMixer.SetFloat(parameter, LinearToDecibel(value));
    }


    private void ApplyVolume(string parameter, Slider slider)
    {
        float value = slider.value;
        PlayerPrefs.SetFloat(parameter, value);
        audioMixer.SetFloat(parameter, LinearToDecibel(value));
        PlayerPrefs.Save();
    }


    private float LinearToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}
