using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RemapKeyCell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public enum InputType
    {
        Keyboard,
        Controller
    }

    [Header("Binding")]
    [SerializeField] private string actionName;
    [SerializeField] private int bindingIndex;
    [SerializeField] private InputType inputType;
    [SerializeField] private bool canRebind = true;

    [Header("References")]
    [SerializeField] private Image backgroundPanel;
    [SerializeField] private TextMeshProUGUI duplicateWarningText;

    [Header("Colors")]
    [SerializeField] private Color normalBackgroundColor = Color.black;
    [SerializeField] private Color highlightBackgroundColor = Color.white;
    [SerializeField] private Color normalTextColor = Color.white;
    [SerializeField] private Color listeningTextColor = new Color(0.5f, 0.85f, 1.0f);

    private Button button;
    private TextMeshProUGUI buttonText;

    public string ActionName => actionName;
    public int BindingIndex => bindingIndex;
    public InputType BindingInputType => inputType;
    public bool CanRebind => canRebind;
    public Button Button => button;

    public Action<RemapKeyCell> PointerEntered;
    public Action<RemapKeyCell> PointerClicked;

    private void Awake()
    {
        button = GetComponent<Button>();
        buttonText = GetComponent<TextMeshProUGUI>();

        SetDuplicateWarning(false);
        RefreshBindingText();
    }

    public void RefreshBindingText()
    {
        if (InputManager.Instance == null || buttonText == null)
        {
            return;
        }

        buttonText.color = normalTextColor;

        buttonText.text = InputManager.Instance.GetBindingDisplayString(
            actionName,
            bindingIndex
        );
    }

    public string GetEffectivePath()
    {
        if (InputManager.Instance == null)
        {
            return string.Empty;
        }

        return InputManager.Instance.GetBindingEffectivePath(
            actionName,
            bindingIndex
        );
    }

    public void SetHighlighted(bool highlighted)
    {
        if (backgroundPanel == null)
        {
            return;
        }

        backgroundPanel.color = highlighted
            ? highlightBackgroundColor
            : normalBackgroundColor;
    }

    public void StartListening()
    {
        if (!canRebind || buttonText == null)
        {
            return;
        }

        buttonText.color = listeningTextColor;
        buttonText.text = ".........";
    }

    public void SetListeningProgress(float progress)
    {
        if (buttonText == null)
        {
            return;
        }

        progress = Mathf.Clamp01(progress);

        const int maximumDots = 9;

        int dotCount = Mathf.CeilToInt(progress * maximumDots);
        dotCount = Mathf.Clamp(dotCount, 1, maximumDots);

        buttonText.text = new string('.', dotCount);
    }

    public void StopListening()
    {
        RefreshBindingText();
    }

    public void SetDuplicateWarning(bool visible)
    {
        if (duplicateWarningText == null)
        {
            return;
        }

        duplicateWarningText.gameObject.SetActive(visible);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PointerEntered?.Invoke(this);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PointerClicked?.Invoke(this);
    }
}