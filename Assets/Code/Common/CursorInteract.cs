using UnityEngine;
using UnityEngine.EventSystems;

public class CursorInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CursorManager.CursorType cursorType;

    private void OnMouseEnter()
    {
        CursorManager.Instance.SetActiveCursorType(cursorType);
    }

    private void OnMouseExit()
    {
        CursorManager.Instance.SetDefaultCursor();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance.SetActiveCursorType(cursorType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.SetDefaultCursor();
    }
}
