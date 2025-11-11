using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class PlayerPrefsEditorTools : MonoBehaviour
{
    [MenuItem("Tools/PlayerPrefs/Clear All PlayerPrefs")]
    private static void ClearPlayerPrefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared!");
    }
}
#endif