using UnityEngine;

public class DemoController : MonoBehaviour
{
    public void DemoButtonHover()
    {
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Combat);
    }

    public void DemoButtonClick()
    {
        GameManager.Instance.LoadSceneByName("Game");
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Wait);
        CursorManager.Instance.LockCursorType(5f);
    }
}
