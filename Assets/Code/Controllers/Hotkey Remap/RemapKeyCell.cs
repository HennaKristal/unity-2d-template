using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

[RequireComponent(typeof(Button), typeof(TextMeshProUGUI))]
public class RemapKeyCell : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private Image backgroundPanel;
    [HideInInspector] public Button button;
    [HideInInspector] public TextMeshProUGUI buttonText;

    [SerializeField] private Color normalBackgroundColor = Color.black;
    [SerializeField] private Color highlightBackgroundColor = Color.white;

    private bool isHighlighted;

    public System.Action<RemapKeyCell> onPointerEntered;
    public System.Action<RemapKeyCell> onPointerClicked;

    private void Start()
    {
        button = GetComponent<Button>();
        buttonText = GetComponent<TextMeshProUGUI>();
    }

    public void SetHighlighted(bool highlighted)
    {
        isHighlighted = highlighted;
        backgroundPanel.color = isHighlighted ? highlightBackgroundColor : normalBackgroundColor;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (onPointerEntered != null)
        {
            onPointerEntered(this);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (onPointerClicked != null)
        {
            onPointerClicked(this);
        }
    }
}
