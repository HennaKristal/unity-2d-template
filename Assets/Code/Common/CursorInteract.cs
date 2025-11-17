using UnityEngine;
using UnityEngine.EventSystems;

public class CursorInteract : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private CursorManager.CursorType cursorType;

    private void OnMouseEnter()
    {
        CursorManager.Instance.SetAciveCursorType(cursorType);
    }

    private void OnMouseExit()
    {
        CursorManager.Instance.SetDefaultCursorType();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        CursorManager.Instance.SetAciveCursorType(cursorType);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        CursorManager.Instance.SetDefaultCursorType();
    }
}
