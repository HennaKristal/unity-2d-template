using UnityEngine;

public class DemoController : MonoBehaviour
{
    public void DemoButtonClick()
    {
        GameManager.Instance.LoadSceneByName("Game");
        CursorManager.Instance.SetActiveCursorType(CursorManager.CursorType.Wait);
        CursorManager.Instance.LockCursorType(2f);
    }
}
