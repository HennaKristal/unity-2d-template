using UnityEngine;

public class Fading : MonoBehaviour
{
    [SerializeField] private Texture2D fadeOutTexture;

    private enum FadeDirection { In, Out, None };
    private FadeDirection fadeDirection = FadeDirection.None;
    private float fadeDuration;
    private float fadeStartValue;
    private float fadeEndValue;
    private float alpha;
    private float fadeStartTime;

    public void StartFadeOut(float _fadeDuration = 1f)
    {
        alpha = 0f;
        fadeStartValue = 0f;
        fadeEndValue = 1f;
        fadeDuration = _fadeDuration;
        fadeDirection = FadeDirection.Out;
        fadeStartTime = Time.time;
    }

    public void StartFadeIn(float _fadeDuration = 1f)
    {
        alpha = 1f;
        fadeStartValue = 1f;
        fadeEndValue = 0f;
        fadeDuration = _fadeDuration;
        fadeDirection = FadeDirection.In;
        fadeStartTime = Time.time;
    }

    private void OnGUI()
    {
        if (fadeDirection == FadeDirection.None)
        {
            return;
        }

        UpdateFade();
    }

    private void UpdateFade()
    {
        float fadeElapsed = Time.time - fadeStartTime;
        alpha = Mathf.Lerp(fadeStartValue, fadeEndValue, fadeElapsed / fadeDuration);

        GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, alpha);
        GUI.depth = -1000;
        GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), fadeOutTexture);

        AudioListener.volume = Mathf.Lerp(fadeEndValue, fadeStartValue, fadeElapsed / fadeDuration);

        if (fadeElapsed >= fadeDuration + 0.1f)
        {
            fadeDirection = FadeDirection.None;
        }
    }
}
