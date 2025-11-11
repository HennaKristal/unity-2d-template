using UnityEngine;

public class CursorInteract : MonoBehaviour
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
}