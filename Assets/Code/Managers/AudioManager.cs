using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private static AudioManager _instance;
    public static AudioManager Instance => _instance;

    [SerializeField] private AudioMixer audioMixer;
    private Slider MusicVolumeSlider;
    private Slider AmbientVolumeSlider;
    private Slider SFXVolumeSlider;
    private Slider DialogueVolumeSlider;
    private Slider UIVolumeSlider;
    private TextMeshProUGUI MusicVolumeSliderText;
    private TextMeshProUGUI AmbientVolumeSliderText;
    private TextMeshProUGUI SFXVolumeSliderText;
    private TextMeshProUGUI DialogueVolumeText;
    private TextMeshProUGUI UIVolumeSliderText;
    private bool isInitializing = true;


    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        GameObject audioPanel = GameObject.Find("AudioOptions");

        MusicVolumeSlider = audioPanel.transform.Find("MusicVolumeSlider").GetComponent<Slider>();
        MusicVolumeSliderText = MusicVolumeSlider.transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        AmbientVolumeSlider = audioPanel.transform.Find("AmbientVolumeSlider").GetComponent<Slider>();
        AmbientVolumeSliderText = AmbientVolumeSlider.transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        SFXVolumeSlider = audioPanel.transform.Find("SFXVolumeSlider").GetComponent<Slider>();
        SFXVolumeSliderText = SFXVolumeSlider.transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        DialogueVolumeSlider = audioPanel.transform.Find("DialogueVolumeSlider").GetComponent<Slider>();
        DialogueVolumeText = DialogueVolumeSlider.transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        UIVolumeSlider = audioPanel.transform.Find("UIVolumeSlider").GetComponent<Slider>();
        UIVolumeSliderText = UIVolumeSlider.transform.Find("ValueLabel").GetComponent<TextMeshProUGUI>();

        LoadVolume("MusicVolume", MusicVolumeSlider, MusicVolumeText);
        LoadVolume("AmbientVolume", AmbientVolumeSlider, AmbientVolumeText);
        LoadVolume("SFXVolume", SFXVolumeSlider, SFXVolumeText);
        LoadVolume("DialogueVolume", DialogueVolumeSlider, DialogueVolumeText);
        LoadVolume("UIVolume", UIVolumeSlider, UIVolumeText);

        isInitializing = false;
    }

    public void OnVolumeSliderChanged()
    {
        if (isInitializing)
        {
            return;
        }

        ApplyVolume("MusicVolume", MusicVolumeSlider, MusicVolumeText);
        ApplyVolume("AmbientVolume", AmbientVolumeSlider, AmbientVolumeText);
        ApplyVolume("SFXVolume", SFXVolumeSlider, SFXVolumeText);
        ApplyVolume("DialogueVolume", DialogueVolumeSlider, DialogueVolumeText);
        ApplyVolume("UIVolume", UIVolumeSlider, UIVolumeText);
    }

    private void LoadVolume(string parameter, Slider slider, TextMeshProUGUI label)
    {
        float value = PlayerPrefs.GetFloat(parameter, 0.8f);
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

    public void SaveAudioSettings()
    {
        PlayerPrefs.Save();
    }

    private float LinearToDecibel(float value)
    {
        return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
    }
}
