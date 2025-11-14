using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public class ClearPlayerPrefs : MonoBehaviour
{
    [MenuItem("Tools/PlayerPrefs/Clear All PlayerPrefs")]
    private static void Execute()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        Debug.Log("PlayerPrefs cleared!");
    }
}
#endif